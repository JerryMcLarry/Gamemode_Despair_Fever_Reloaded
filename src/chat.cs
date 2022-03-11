datablock AudioProfile(DespairChatSound)
{
	fileName = $Despair::Path @ "res/sounds/chat.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock AudioProfile(DespairAdminChatSound)
{
	fileName = $Despair::Path @ "res/sounds/adminchat.wav";
	description = Audio2d;
	preload = true;
};

datablock AudioProfile(DespairAdminBwoinkSound)
{
	fileName = $Despair::Path @ "res/sounds/bwoink.wav";
	description = Audio2d;
	preload = true;
};

datablock ItemData(DespairEmptyFloatItem)
{
	shapeFile = "base/data/shapes/empty.dts";
	gravityMod = 0;
	canPickup = 0;
};

function DespairEmptyFloatItem::onPickup() {}

function serverCmdMe(%client, %m1, %m2, %m3, %m4, %m5, %m6, %m7, %m8, %m9, %m10, %m11, %m12, %m13, %m14, %m15, %m16, %m17, %m18, %m19, %m20, %m20, %m22, %m23, %m24)
{
	if (!isObject(%pl = %client.player) || (%pl.unconscious && !%pl.currResting))
		return;

	if($DespairTrialOpening && %client != $DespairTrialCurrSpeaker)
		return;

	if($Sim::Time - %client.lastEmoteTime < $chatDelay)
		return;

	if($Sim::Time < %client.timeOut) //Special trial ability
	{
		messageClient(%client, '', '\c5You\'re unable to act\c6!');
		return;
	}

	if(%pl.unconscious && !%pl.forcedEmote)
		return;

	%pl.forcedEmote = "";

	%text = %m1;
	for (%i=2; %i<=24; %i++)
		%text = %text SPC %m[%i];
	%text = trim(stripMLControlChars(%text));
	if (%text $= "")
		return;
	
	%client.lastSpeakTime = $Sim::Time;
	%client.lastEmoteTime = $Sim::Time;
	%name = getCharacterName(%client.character, $despairTrial);

	if(!$despairTrial)
	{
		%time = getDayCycleTime();
		%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
		%time = %time - mFloor(%time); //get rid of excess stuff

		%time = getDayCycleTimeString(%time, 1);

		%time = "D" @ $days @ "|" @ %time;
	}
	else
	{
		%time = getTimeString(mFloor($Sim::Time - $DespairTrial));
	}

	//echo("-+ (ACTION) " @ %name SPC %text);
	RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") emoted '" @ %text @ "'", "\c1");

	%count = ClientGroup.getCount();
	for (%i = 0; %i < %count; %i++)
	{
		%_name = %name;
		%other = ClientGroup.getObject(%i);
		if (%other.miniGame == $DefaultMiniGame && isObject(%other.player))
		{
			if(%other.player.unconscious)
				continue;
			if (vectorDist(%a = %other.player.getEyePoint(), %b = %pl.getEyePoint()) > 24) //Out of range
				continue;
			if(%ray = containerRayCast(%a, %b, $TypeMasks::FxBrickObjectType, %pl))
				%_name = "Someone";
		}

		messageClient(%other, '', '\c7[%1] <color:ffff80>%2 %3', %time, %_name, %text);
	}
}

package DespairChat
{
	function serverCmdStartTalking(%client)
	{
		if (%client.miniGame != $DefaultMiniGame)
			Parent::serverCmdStopTalking(%client);
	}
	function serverCmdMessageSent(%client, %text)
	{
		%player = %client.player;

		if (%client.miniGame != $DefaultMiniGame && %client.hasSpawnedOnce)
			return Parent::serverCmdMessageSent(%client, %text);

		%text = getSubStr(trim(stripMLControlChars(%text)), 0, $Pref::Server::MaxChatLen);

		if (%text $= "")
			return;

		if(DespairSpecialChat(%client, %text))
			return;

		if($Sim::Time - %client.lastSpeakTime < $chatDelay)
		{
			messageClient(%client, '', '\c5Slow down\c6!');
			return;
		}

		if(!$despairTrial)
		{
			%time = getDayCycleTime();
			%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
			%time = %time - mFloor(%time); //get rid of excess stuff

			%time = getDayCycleTimeString(%time, 1);

			%time = "D" @ $days @ "|" @ %time;
		}
		else
		{
			%time = getTimeString(mFloor($Sim::Time - $DespairTrial));
		}
		%name = %client.getPlayerName();
		if (%client.killerHelper)
		{
			serverCmdTeamMessageSent(%client, %text);
			return;
		}
		if (!isObject(%player))
		{
			%client.lastSpeakTime = $Sim::Time;
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%member = ClientGroup.getObject(%i);

				if ((!isObject(%member.player) && !%member.killerHelper) || isEventPending($DefaultMiniGame.restartSchedule) || %member.miniGame != $DefaultMiniGame)
				{
					messageClient(%member, '', '\c7[%1] <color:808080>%2<color:b0b0b0>: %3', %time, %name, %text);
				}
				$lastDeadText = %text; //for medium
			}
			//echo("-+ (DEAD) " @ %name @ ": " @ %text);
			RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") deadchat '" @ %text @ "'", "\c1");
			return;
		}
		if(%player.unconscious)
			return;

		%player.playThread(3, "talk");
		%player.schedule(strLen(%text) * 35, "playThread", 3, "root");

		%name = getCharacterName(%client.character, $despairTrial);

		%sound = DespairChatSound;
		%type = "says";
		%range = 32;
		%wall_effect = 6;

		if(%player.health <= 0) //critical health
		{
			%type = "stammers";
			%range = 8;
			%text = stutterText(%text, 0.05);
		}
		if(getSubStr(%text, 0, 1) $= "@") //Whispering
		{
			%text = getSubStr(%text, 1, strLen(%text));
			%type = "whispers";
			%range = 4;
			if (%text $= "")
				return;
		}

		if(%player.character.trait["Nervous"])
		{
			%text = stutterText(%text);
		}

		if(%player.character.trait["Loudmouth"])
		{
			%range += 3;
		}
		else if(%player.character.trait["Softspoken"])
		{
			%range *= 0.8;
			%text = softSpeakText(%text);
		}

		if(isObject(%img = %player.getMountedImage(0)) && %img == nameToID(radioImage) && (%slot = %player.findTool("RadioItem")) != -1)
		{
			%sound = radioTalkSound;
			%type = "radios";
			%range = 16;
			%props = %player.getItemProps(%slot);
			radioTransmitMessage(%player, %props.channel, %text);
		}

		if(%type $= "says")
		{
			serverPlay3D(%sound, %player.getHackPosition());

			%shape = new Item()
			{
				datablock = DespairEmptyFloatItem;
				position = VectorAdd(%player.position, "0 0 2");
			};
			%shape.noExamine = true;
			%shape.setCollisionTimeout(%player);
			%shape.setShapeName(%text);
			%shape.setShapeNameDistance(%range);
			%shape.setVelocity("0 0 0.5");
			%shape.deleteSchedule = %shape.schedule(3000, delete);
		}
		%client.lastSpeakTime = $Sim::Time;
		//echo("-+ " @ %name @ " (" @ %client.getPlayerName() @ "): " @ %text);
		RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") " @ %type @ " '" @ %text @ "'", "\c1");
		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%member = ClientGroup.getObject(%i);
			if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
			{
				if(%member.miniGame != $DefaultMiniGame && %member.isAdmin)
					messageClient(%member, '', '\c7[%1] <color:ffff80>%2 %3<color:fffff0>, %4', %time, %name SPC "(" @ %client.getPlayerName() @ ")", %type, %text);
				else
					messageClient(%member, '', '\c7[%1] <color:ffff80>%2 %3<color:fffff0>, %4', %time, %name, %type, %text);
				continue;
			}
			%_name = %name;
			%_text = %text;
			%c1 = "ffff80";
			%c2 = "fffff0";

			%a = %player.getEyePoint();
			%b = %member.player.getEyePoint();

			%distance = vectorDist(%a, %b) + %wall_effect * getWallsBetween(%a, %b);

			if ((%realdist = vectorDist(%a, %b)) > %range)
				continue;

			if ($despairTrial $= "")
			{
				%factor = getMax(0, %distance / (%range*4) - 0.15);
				%_text = scrambleText(%text, %factor);
				if(%factor > 0.4)
					%_name = "Someone";

				%distfactor = %realdist / %range;
				if(%distfactor > 0.5)
				{
					%c1 = "f2f255";
					%c2 = "f2f2c0";
				}
				if(%distfactor > 0.65)
				{
					%c1 = "e6e62e";
					%c2 = "e6e693";
				}
				if(%distfactor > 0.8)
				{
					%c1 = "d9d90b";
					%c2 = "d9d96a";
				}
			}

			if(%member.player.unconscious && !%member.player.currResting)
			{
				%_name = "Someone";
				if(%type !$= "whispers") //whispering to someone sleeping is LOUD AND CLEAR
					%_text = scrambleText(%text, 0.5);
			}
	
			messageClient(%member, '', '\c7[%1] <color:%5>%2 %3<color:%6>, %4', %time, %_name, %type, %_text, %c1, %c2);
		}
	}
	function serverCmdTeamMessageSent(%client, %text) //Adminchat & KillerChat
	{
		%isLegit = !%client.isAdmin && !isObject(%client.player) && %client.killerHelper;
		if(!%client.isAdmin && !%client.killer && !%isLegit)
			return;
		if (%text $= "")
			return;
		%name = %client.getPlayerName();

		if(%client.killer && !%client.isAdmin)
		{
			%killer = true;
			%name = "Killer";
		}
		if(getSubStr(%text, 0, 1) $= "@") //Killer chat
		{
			%text = getSubStr(%text, 1, strLen(%text));
			%killer = true;
		}
		if(%isLegit)
			%killer = true;

		//echo("-+ (" @ (%killer ? "KILLER" : "ADMIN") @ ") " @ %client.getPlayerName() @ ": " @ %text);
		RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") used " @ (%killer ? "killer" : "admin") @"chat '" @ %text @ "'", "\c2");

		for (%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%member = ClientGroup.getObject(%i);
			if(%member.isAdmin || (!isObject(%member.player) && %member.killerHelper))
			{
				if(%killer)
				{
					//if(isObject(%member.player) && %member.miniGame == $DefaultMiniGame)
					//	continue;
					messageClient(%member, '', '\c2--[<color:FF8080>%1<color:FFF0F0>: %2', %name, %text);
				}
				else
					messageClient(%member, '', '\c2--[<color:80FF80>%1<color:F0FFF0>: %2', %name, %text);
				%member.play2d(DespairAdminChatSound);
			}
		}
		if(%killer && isObject($pickedKiller))
		{
			messageClient($pickedKiller, '', '\c2--[ADMIN]<color:FF8080>%1<color:FFF0F0>: %2', "Admin", %text);
			$pickedKiller.play2d(DespairAdminChatSound);
		}
	}
};

activatePackage("DespairChat");


function getWallsBetween(%pos, %end)
{
	%count = 0;

	while (vectorDist(%pos, %end) > 1 && %count < 32)
	{
		%ray = containerRayCast(%pos, %end, $TypeMasks::FxBrickObjectType, %exempt);

		if (!%ray)
			break;

		%count++;

		%exempt = getWord(%ray, 0);
		%pos = getWords(%ray, 1, 3);
	}

	return %count;
}

function scrambleText(%text, %factor, %replace, %retain)
{
	if (%factor $= "")
		%factor = 0.2;
	if (%replace $= "")
		%replace = "#";
	if (%retain $= "")
		%retain = " .,!?";

	%length = strlen(%text);

	for (%j = 0; %j < %length; %j++)
	{
		%char = getSubStr(%text, %j, 1);

		if ((%retain !$= "" && strpos(%retain, %char) != -1) || getRandom() > %factor)
			%result = %result @ %char;
		else
			%result = %result @ %replace;
	}

	return %result;
}

function softSpeakText(%text)
{
	if (%text $= "")
		return;
	%result = %text;//strlwr(%text);
	for (%i=0;%i<strlen(%text);%i++)
	{
		if ((%char = getSubStr(%result, %i, 1)) $= " ") //space character
			continue;
		if (%char $= "." || %char $= "!" || %char $= "?")
		{
			%result = getSubStr(%result, 0, %i+1) @ ".." @ getSubStr(%result, getMin(%i+2, strlen(%result)), strlen(%result));
			%i += 2;
		}
	}
	if(getSubStr(%result, strlen(%result)-1, 1) !$= ".")
		%result = %result @ "...";
	return %result;
}

function stutterText(%text, %prob)
{
	if (%text $= "")
		return;
	if (%prob $= "")
		%prob = 0.1;
	if (%prob <= 0)
		return %text;
	%result = %text;
	for (%i=0;%i<strlen(%result);%i++)
	{
		if (strpos("bcdfghjklmnpqrstvxzwy", %char = getSubStr(%result, %i, 1)) == -1) //incompatible
			continue;
		if (getRandom() < %prob)
			%result = getSubStr(%result, 0, %i) @ %char @ %char @ getSubStr(%result, %i+1, strlen(%result));
	}
	return %result;
}