function GameConnection::examineObject(%client, %col)
{
	if(%col.noExamine)
		return;
	%player = %client.player;
	%character = %client.character;
	%text = "<font:cambria:24><color:FFFFFF>";

	if(%col.getType() & ($TypeMasks::playerObjectType | $TypeMasks::CorpseObjectType))
	{
		%name = getCharacterName(%col.character, $despairTrial);
		%text = %text @ "This is \c3" @ %name;
		%gender = %col.character.gender;
		if (%col.isDead)
		{
			if(!%client.killer && isObject(%player))
				DespairCheckInvestigation(%player, %col);

			%ref = %gender $= "female" ? "She's" : "He's";
			if(%col.mangled)
				%text = %text @ "\n\c0They are horribly mangled!";
			else
				%text = %text @ "\n\c0" @ %ref @ " dead.";

			if (%col.suicide && !%col.mangled)
				%text = %text @ "\n\c5It was suicide...";
			if(isObject(%player) && %character.trait["investigative"] && !%col.mangled)
			{
				%day = %col.attackDay[%col.attackCount];

				%tod = %col.attackDayTime[%col.attackCount];
				%tod += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
				%tod = %tod - mFloor(%tod); //get rid of excess stuff

				if(%player.margin[1] $= "")
					%player.margin[1] = getRandom(1, 5) * 0.01;
				%tod1 = getDayCycleTimeString(getMax(%tod - %player.margin[1], 0), 1);
				%mod12 = getWord(%tod1, 1);
				%tod1 = getWord(%tod1, 0) SPC (%mod12 $= "PM" ? "<color:7e7eff>" : "<color:ffbf7e>") @ %mod12;

				if(%player.margin[2] $= "")
					%player.margin[2] = getRandom(1, 5) * 0.01;
				%tod2 = getDayCycleTimeString(%tod + %player.margin[2], 1);
				%mod12 = getWord(%tod2, 1);
				%tod2 = getWord(%tod2, 0) SPC (%mod12 $= "PM" ? "<color:7e7eff>" : "<color:ffbf7e>") @ %mod12;

				%when = "Day" SPC %day;
				// if(%day == $days)
				// 	%when = "today";
				// else if(%day == $days - 1)
				// 	%when = "yesterday";
				// else
				// 	%when = "long ago";

				%text = %text @ "\n\c6" @ "They died \c3" @ %when @ " \c6between\c5" SPC %tod1 SPC "\c6and\c5" SPC %tod2 @ ".";
				for(%i=0;%i<=%col.attackCount;%i++)
				{
					%wounds[%col.attackType[%i]]++;
				}
				if(%wounds["blunt"] > 0)
				{
					%field = %wounds["blunt"] SPC "bruises";
					%fields = setField(%fields, getFieldCount(%fields), %field);
				}
				if(%wounds["sharp"] > 0)
				{
					%field = %wounds["sharp"] SPC "cuts";
					%fields = setField(%fields, getFieldCount(%fields), %field);
				}
				if(%wounds["gun"] > 0)
				{
					%field = %wounds["gun"] SPC "bullet holes";
					%fields = setField(%fields, getFieldCount(%fields), %field);
				}

				//Tissue damage comes first
				if(getFieldCount(%fields) > 0)
					%text = %text @ "\n\c6" @ "They have \c3" @ naturalGrammarList(%fields) @ "\c6.";

				if(%wounds["choking"] > 0)
				{
					%text = %text @ "\n\c6" @ "There is \c3bruising on their neck\c6.";
					%haswounds = true;
				}

				if(%col.attackType[%col.attackCount] $= "fall")
				{
					%text = %text @ "\n\c6" @ "They \c3fell to their death\c6.";
					%haswounds = true;
				}
				else if(%wounds["fall"] > 0)
				{
					%text = %text @ "\n\c6" @ "There are \c3signs of falling\c6.";
					%haswounds = true;
				}

				if(%col.attackType[%col.attackCount] $= "bleed")
				{
					%text = %text @ "\n\c6" @ "They have \c3bled to death\c6.";
					%haswounds = true;
				}
				else if(%wounds["bleed"] > 0)
				{
					%text = %text @ "\n\c6" @ "Their wounds had them \c3bleeding\c6.";
					%haswounds = true;
				}

				if(getFieldCount(%fields) <= 0 && !%haswounds)
					%text = %text @ "\n\c6" @ "They have \c3no visible wounds\c6.";
			}
		}
		else
		{
			if(%col.unconscious)
			{
				%text = %text @ "\n\c5" @ (%gender $= "female" ? "She's" : "He's") @ " sleeping.";
			}
			
			if(%col.IsBloody())
			{
				%text = %text @ "\n\c0" @ (%gender $= "female" ? "She's" : "He's") @ " bloody.";
			}

			if(isObject(%img = %col.getMountedImage(1)))
			{
				%text = %text @ "\n\c6" @ (%gender $= "female" ? "She" : "He") @ " is wearing a \c3" @ %img.item.uiName;
			}

			//HONESTLY FUCK INJURIES AIGHT
			//if(!$despairTrial && vectorDist(%player.getPosition(), %col.getPosition()) < 2) //practically hug 'em, also no trial stuff
			//{
			//	if((!isObject(%img) || !%img.item.hideAppearance) && %col.health/%col.maxHealth <= 0.9) //Coats obscure injuries, injuries are only displayed at 90% health
			//		%text = %text @ "\n\c6On closer inspection, they are \c0injured\c6!";
			//}
		}
		if(isObject(%img = %col.getMountedImage(0)))
			%text = %text @ "\n\c6" @ (%gender $= "female" ? "She" : "He") @ " has a \c3" @ %img.item.uiName;
	}

	if(%col.getType() & $TypeMasks::itemObjectType)
	{
		%name = %col.getDataBlock().uiName;
		%props = %col.itemProps;

		if(%props.class $= "PaperProps")
		{
			%name = %props.name;
			%b = %props.contents;
		}
		if(%props.class $= "KeyProps")
		{
			%name = %props.name;
		}
		if(%props.class $= "MeleeProps")
		{
			if(%props.bloody)
				%b = "\c0It's bloody.";
		}
		%text = %text @ "This is " @ aOrAn(%name) @ " \c3" @ %name @ "\n" @ %b;
	}

	if(%col.getType() & $TypeMasks::StaticShapeObjectType)
	{
		if(%col.getDataBlock().getID() == nameToID("writingDecal"))
		{
			%text = %text @ "This is a " @ (%col.isBlood ? "\c0bloody" : "") SPC "scribble.";
			%text = %text @ "\n" @ %col.contents;
		}
		else if(%col.getDataBlock().getID() == nameToID("strandDecal"))
		{
			%text = %text @ "This is a " @ "<color:" @ rgbToHex(vectorScale(getWords(%col.color, 0, 2), 255)) @ ">strand.";
		}
		else
		{
			return;
		}
	}

	commandToClient(%client, 'CenterPrint', %text, 5);
}
