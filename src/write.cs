datablock audioProfile(WriteSound)
{
	fileName =  $Despair::Path @ "res/sounds/write.wav";
	description = AudioQuiet3d;
	preload = true;
};

datablock StaticShapeData(writingDecal)
{
	shapeFile = $Despair::Path @ "res/shapes/writing.dts";
	canClean = true;
};

function serverCmdWrite(%client, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15, %a16, %a17, %a18, %a19, %a20, %a20, %a22, %a23, %a24)
{
	if(!isObject(%player = %client.player))
		return;

	%text = %a1;
	for (%i=2; %i<=24; %i++)
		%text = %text SPC %a[%i];
	%text = strreplace(%text, "<br>", "[br]");
	%text = strreplace(%text, "<jl>", "[jl]");
	%text = strreplace(%text, "<jc>", "[jc]");
	%text = strreplace(%text, "<jr>", "[jr]");
	%text = trim(stripMLControlChars(%text));
	if (%text $= "")
		return;

	%text = strreplace(%text, "[br]", "\n");
	%text = strreplace(%text, "[jl]", "<just:left>");
	%text = strreplace(%text, "[jc]", "<just:center>");
	%text = strreplace(%text, "[jr]", "<just:right>");

	if(%player.health <= 0) //Critical state
	{
		%text = scrambleText(%text, mClampF(strlen(%text) * 0.01, 0.1, 0.9));

		%a = %player.getEyePoint();
		%b = vectorAdd(%a, vectorScale(%player.getEyeVector(), 6));
		%mask =	$SprayBloodMask;
		%ray = containerRayCast(%a, %b, %mask, %player);
		if(isObject(%ray))
		{
			messageClient(%client, '', "\c5You use the last of your strength to write your final message...");
			if(!%player.suicide)
				%client.AddPoints(2);
			%rayPosition = getWords(%ray, 1, 3);
			%rayNormal = getWords(%ray, 4, 6);
			%rayPosition = VectorAdd(%rayPosition, VectorScale(%rayNormal, 0.01));
			%forward = vectorScale(%player.getForwardVector(), getWord(%rayNormal, 2));
			%angle = mATan(getWord(%forward, 0), getWord(%forward, 1));
			%color = 0.75 + 0.1 * getRandom() @ " 0 0 1";
			%decal = spawnDecal(writingDecal, %rayPosition, %rayNormal, 1, %color, %angle, "", 1);
			%decal.spillTime = $Sim::Time;
			%decal.freshness = 1;
			%decal.contents = "\c0" @ %text;
			%decal.isBlood = true;
			%decal.source = %player;
			%player.bloody["rhand"] = true; //They used their hand to write the message
			%player.applyAppearance();
			%player.health = $Despair::CritThreshold;
			%player.critLoop();
			RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") used /write '" @ %text @ "'", "\c2");
		}
		else
		{
			messageClient(%client, '', "\c5Look at a surface!!");
		}
		return;
	}

	if((%slot = %player.findTool("PenItem")) != -1)
	{
		%props = %player.getItemProps(%slot);
		if(%props.ink <= 0)
		{
			messageClient(%client, '', "\c5Your pen ran out of ink!");
			%pen = false;
		}
		else
		{
			%pen = true;
			%prob = getMax(0, 1 - ((%props.ink*2)/%props.maxink));
			%text = scrambleText(%text, %prob);
		}
	}
	
	else if(%player.bloodyWriting > 0 && %player.bloody["rhand"] = true)
	{
		%blood = true;
		%text = scrambleText(%text, 0.2);
		%player.bloodyWriting--;
	}


	if (%player.tool[%player.currTool] == nameToID(PaperItem))
	{
		%props = %player.getItemProps();
		%color = %pen ? "\c1" : (%blood ? "\c0" : "\c6");
		if(%props.name $= "Daily News")
			messageClient(%client, '', "\c5You are unable to write on this paper!");
		else
		{
			%props.contents = %props.contents @ %color @ %text;
			if(!%blood)
				serverPlay3d("WriteSound", %player.getHackPosition());
		}
		PaperImage.onMount(%player, 0);
		return;
	}

	%a = %player.getEyePoint();
	%b = vectorAdd(%a, vectorScale(%player.getEyeVector(), 6));

	%mask = $SprayBloodMask;

	%ray = containerRayCast(%a, %b, %mask, %player);

	if(isObject(%ray))
	{
		if (%ray.getType() & $TypeMasks::ItemObjectType)
		{
			if (%ray.getDataBlock() == nameToID(PaperItem))
			{
				%props = %ray.itemProps;
				%color = %pen ? "\c1" : (%blood ? "\c0" : "\c6");
				if(%props.name $= "Daily News")
					messageClient(%client, '', "\c5You are unable to write on this paper!");
				else
				{
					%props.contents = %props.contents @ %color @ %text;
					RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") used /write '" @ %text @ "'", "\c2");
				}
				return;
			}
		}
		else
		{
            if(!%blood && !%pen)
            {
                messageClient(%client, '', "\c5You need something to write with!");
                return;
            }
			%rayPosition = getWords(%ray, 1, 3);
			%rayNormal = getWords(%ray, 4, 6);
			%rayPosition = VectorAdd(%rayPosition, VectorScale(%rayNormal, 0.01));
			%forward = vectorScale(%player.getForwardVector(), getWord(%rayNormal, 2));
			%angle = mATan(getWord(%forward, 0), getWord(%forward, 1));
			%color = %pen ? "0 0 1 1" : (0.75 + 0.1 * getRandom() @ " 0 0 1");
			%decal = spawnDecal(writingDecal, %rayPosition, %rayNormal, 1, %color, %angle, "", 1);
			%decal.color = %color;
			%decal.spillTime = $Sim::Time;
			%decal.freshness = 1;
			%decal.contents = (%pen ? "\c1" : "\c0") @ %text;
			%decal.source = %player;
			RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") used /write '" @ %text @ "'", "\c2");
			if(%blood)
				%decal.isBlood = true;
            else
                serverPlay3d("WriteSound", %player.getHackPosition());
			if(%pen)
			{
				%props.ink--; //More ink consumed
			}
		}
	}
}
//Shorter version
function serverCmdW(%client, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15, %a16, %a17, %a18, %a19, %a20, %a20, %a22, %a23, %a24)
{
	serverCmdWrite(%client, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13, %a14, %a15, %a16, %a17, %a18, %a19, %a20, %a20, %a22, %a23, %a24);
}

function serverCmdSign(%client)
{
	//Sign the paper with initials. Cannot be changed.
}