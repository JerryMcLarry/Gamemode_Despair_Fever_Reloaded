$Despair::Traits::Tick = 3000; //miliseconds

$Despair::Traits::Positive = "Investigative	Heavy Sleeper	Gang Member	Extra Tough	Bodybuilder	Athletic	Loudmouth	Optimistic"; //"Pickpocket Medium"
$Despair::Traits::Neutral = "Snorer	Feel No Pain	Hatter	Chain Smoker	Apathetic";
$Despair::Traits::Negative = "Clumsy	Paranoid	Nervous	Frail	Cold	Sluggish	Hemophiliac	Squeamish	Softspoken	Social Anxiety	Mood Swings	Melancholic	Schizo"; //Narcoleptic

//positive
$Despair::Traits::Description["Investigative"] = "You will get more information from corpses.";
$Despair::Traits::Description["Heavy Sleeper"] = "Can sleep on any surface without suffering sore back!";
$Despair::Traits::Description["Gang Member"] = "Totally gangsta. Has a cool hat and some lockpicks.";
$Despair::Traits::Description["Extra Tough"] = "More resistant to weapon damage!";
$Despair::Traits::Description["Bodybuilder"] = "Faster weapon swings!";
$Despair::Traits::Description["Athletic"] = "Slightly faster run speed!";
$Despair::Traits::Description["Loudmouth"] = "Always the attention seeker, your voice travels for quite the distance.";
$Despair::Traits::Description["Optimistic"] = "Nothing will make you feel depressed!";
$Despair::Traits::Description["Medium"] = "You're able to listen in on what the dead is whispering about beyond the veil. [Beta]";

//disabled
$Despair::Traits::Description["Chekhov's Gunman"] = "Guns have been banned for a long time. Why do you have this trait?";
$Despair::Traits::Description["Pickpocket"] = "You're not supposed to have this... you sneaky thief.";

//neutral
$Despair::Traits::Description["Snorer"] = "Snore loudly when sleeping.";
$Despair::Traits::Description["Feel No Pain"] = "No pain effects!";
$Despair::Traits::Description["Hatter"] = "Spawn with a random hat in your room!";
$Despair::Traits::Description["Chain Smoker"] = "Cough constantly.";
$Despair::Traits::Description["Apathetic"] = "Completely unaffected by mood.";

//negative
$Despair::Traits::Description["Clumsy"] = "Trip on blood and dropped items, chance to drop held item when tripping!";
$Despair::Traits::Description["Paranoid"] = "Constantly alert. Never able to get a good night's rest.";
$Despair::Traits::Description["Nervous"] = "Stuttered speech, easily stressed out.";
$Despair::Traits::Description["Frail"] = "Less health.";
$Despair::Traits::Description["Cold"] = "Constantly ill...";
$Despair::Traits::Description["Sluggish"] = "Slightly slower run speed.";
$Despair::Traits::Description["Hemophiliac"] = "Bleed more.";
$Despair::Traits::Description["Squeamish"] = "Blood makes you scream! Seeing corpses will make you faint.";
$Despair::Traits::Description["Softspoken"] = "Quieter speech, unable to use caps...";
$Despair::Traits::Description["Social Anxiety"] = "Being near (living) people for too long makes you freak out and faint.";
$Despair::Traits::Description["Mood Swings"] = "Your mood is swayed a lot easier.";
$Despair::Traits::Description["Melancholic"] = "You just can't ever feel happy.";
$Despair::Traits::Description["Narcoleptic"] = "Random bouts of lethargy leaves you unable to complete most tasks. [Beta]";
$Despair::Traits::Description["Schizo"] = "Your mind frequently wanders, and you'll never be sure if you're truly alone.";

			function cleanLethargic()
			{
				%obj.setStatusEffect($SE_passiveSlot, "lethargic_clearer");
			}
function GenerateTraits(%character, %client)
{
	%typePositive = $Despair::Traits::Positive;
	%typeNeutral = $Despair::Traits::Neutral;
	%typeNegative = $Despair::Traits::Negative;
	%traitCount = getRandom(1, 2); //In actuality this decides how many positive-negative trait combos there are
	%neutralCount = getRandom(1, 2);
	while(%traitCount-- >= 0)
	{
		%lastType = %typeStr;
		%lastTrait = %trait;
		if(%neutralCount > 0)
		{
			%neutralCount--;
			%traitCount++;
			%typeStr = "neutral";
		}
		else if(%typeStr $= "positive")
			%typeStr = "negative";
		else
		{
			%typeStr = "positive";
			%traitCount++; //Positive-Negative combinations count as a single trait essentialy
		}
		// if(%typeStr $= "positive" && !isObject($gunmanChar) && getRandom() <= 0.1)
		// {
		// 	%trait = "Chekhov's Gunman";
		// 	$gunmanChar = %character;
		// }
		// else
		// {
			%trait = getField(%type[%typeStr], %index = getRandom(0, getFieldCount(%type[%typeStr]) - 1));
			%type[%typeStr] = removeField(%type[%typeStr], %index);
		// }

		//Check if we picked conflicting traits
		if(checkTraitConflicts(%character.traitList, %trait))
		{
			%trait = %lastTrait; //rollback a bit, we still need a negative
			%typeStr = %lastType;
			%traitCount++;
			continue;
		}
		%character.trait[%trait] = true;
		%character.traitList = setField(%character.traitList, getFieldCount(%character.traitList), %trait);

		%color = %typeStr $= "positive" ? "\c2" : (%typeStr $= "negative" ? "\c0" : "\c6");
		%desc = $Despair::Traits::Description[%trait];
		if(isObject(%client))
			messageClient(%client, '', '\c5You now have %1%2\c5 trait! %3', %color, %trait, %desc);
	}
	if(isObject(%client))
		messageClient(%client, '', '\c5Say \c3/traits\c5 to bring up your traits again.');
}

function Player::traitSchedule(%obj)
{
	cancel(%obj.traitSchedule);
	if(!$Despair::Traits::Enabled)
		return;
	if(%obj.getState() $= "Dead")
		return;
	if($despairTrial !$= "") //It's very annoying to cough during trials let's face it
		return;

	if(!%obj.client.killer && !%obj.unconscious && !isEventPending(%obj.passOutSchedule))
	{
		if((%se = %obj.statusEffect[$SE_sleepSlot]) $= "exhausted" && getRandom() < 0.05)
		{
			messageClient(%obj.client, '', "\c5You're about to pass out due to your \c3exhaustion\c5...");
			%obj.passOutSchedule = %obj.schedule(1000, knockOut, 90);
		}
	}
	if(%obj.character.trait["Squeamish"])
	{
		%ges = %obj.getEnvironmentStress();
		%stress = getWord(%ges, 0);
		%foundCorpse = getWord(%ges, 1);

		if(%obj.client.killer)
			%foundCorpse = ""; //what corpse?

		if(isObject(%foundCorpse) && !%foundcorpse.checkedBy[%obj])
			serverCmdAlarm(%obj.client, 1); //very easy (and lazy) way of doing this. despairCheckInvestigation has Squeamish check for fainting, too.
		else if(%stress && getRandom() < 0.03 * %stress)
		{
			%high = -1;
			%text[%high++] = "hyperventilates!";
			%text[%high++] = "freaks out!";
			%text[%high++] = "gasps!";
			%text = %text[getRandom(%high)];
			serverCmdMe(%obj.client, %text);
		}
	}
	if(%obj.character.trait["Social Anxiety"] && %obj.unconscious)
	{
		%center = %obj.getEyePoint();
		initContainerRadiusSearch(%center, 48, $TypeMasks::PlayerObjectType);
		while (isObject(%found = containerSearchNext()))
		{
			if(%found == %obj || %found.getState() $= "Dead")
				continue;
			%point = %found.getEyePoint();
			%ray = containerRayCast(%center, %point, $TypeMasks::FxBrickObjectType, %obj);

			if(!isObject(%ray) && %obj.isWithinView(%point)) //So you can't "detect presence" of the killer sneaking up on you or something.
			{
				%stress++; //More people = more stress
			}
		}

		if(%stress)
		{
			if(!%obj.client.killer &&  !%obj.unconscious && !isEventPending(%obj.passOutSchedule) && %obj.anxiety > 4 && $Sim::Time - %obj.lastKO > 60)
			{
				messageClient(%obj.client, '', "\c5You're about to pass out due to your \c3social anxiety\c5...");
				%obj.passOutSchedule = %obj.schedule(5000, knockOut, 20);
				%obj.anxiety = 0;
			}
			else if(getRandom() < 0.1 * %stress)
			{
				if($Sim::Time - %obj.client.lastEmoteTime >= 60)
				{
					%obj.anxiety = 0;
				}
				%obj.anxiety++;
				if(%obj.anxiety == 1)
					%level = "\c3a bit ";
				else if(%obj.anxiety == 2)
				{
					%level = "\c3pretty ";
					if ($Sim::Time - %obj.lastMoodChange > 30)
						%obj.addMood(-2);
				}
				else if(%obj.anxiety >= 3)
				{
					%level = "\c0very ";
					if ($Sim::Time - %obj.lastMoodChange > 30)
						%obj.addMood(-5);
				}

				if (%obj.anxiety >= 2)
				{
					%high = -1;
					%text[%high++] = "is trembling...";
					%text[%high++] = "blushes.";
					%text[%high++] = "fidgets nervously.";
					%text[%high++] = "is sweating profusely.";
					%text[%high++] = "clears their throat.";
					%text[%high++] = "has their hands twitching uncontrollably.";
					%text = %text[getRandom(%high)];
					serverCmdMe(%obj.client, %text);
				}
				messageClient(%obj.client, '', '\c5You\'re feeling %1anxious...', %level);
			}
		}
	}
	if(%obj.character.trait["Schizo"])
	{
		%random = GameCharacters.getObject(getRandom(0, GameCharacters.getCount()-1));
		if(getRandom() < 0.03)
		{
			%dream = getDreamText();
			if (getRandom() < 0.15 && isObject(%random)) //less chance for a random character name to appear
				%dream = %random.name;
			messageClient(%obj.client, '', '   \c1... %1 ...', %dream);
		}
       else if(getRandom() < 0.05)
        {
            %high = -1;
            %type[%high++] = "whispers";
            %type[%high++] = "says";
            %type = %type[getRandom(%high)];
            %time = getTimeString(mFloor($Sim::Time - $DespairTrial));
			%time = "D" @ $days @ "|" @ %time;
            messageClient(%obj.client, '', '\c7[%1]<color:ffff80>%2 %3<color:fffff0>, %4', %time, " Someone", %type, getSchizoText());
        }
	}
	if(%obj.character.trait["Medium"])
	{
		if(getRandom() < 0.03)
		{
			%information = getPaperTips();
			messageClient(%obj.client, '', '   \c1... %1 ...', %information);
		}
		if(getRandom() < 0.01)
		{
			%character = GameCharacters.getObject(getRandom(0, GameCharacters.getCount() - 1));
			%trait = getField(%character.traitList, getRandom(0, getFieldCount(%character.traitList) - 1));
			%msg = getWord(%character.name, 0) @ " is " @ aOrAn(%trait) SPC %trait SPC " kind of person!";
			messageClient(%obj.client, '', '   \c1... %1 ...', %msg);
		}
	}
	if(%obj.character.trait["Cold"])
	{
		if(getRandom() < 0.015)
		{
			%high = -1;
			%text[%high++] = "sneezes!";
			%text[%high++] = "sniffs!";
			%text[%high++] = "coughs!";
			%text = %text[getRandom(%high)];
			serverCmdMe(%obj.client, %text);
			if(getRandom() < 0.1)
				%obj.addMood(-1);
		}
	}
	if(%obj.character.trait["Chain Smoker"])
	{
		if(getRandom() < 0.015)
		{
			serverCmdMe(%obj.client, "coughs!");
			if(getRandom() < 0.1)
				%obj.addMood(-1);
		}
	}
	if(%obj.character.trait["Narcoleptic"])
	{
		if(getRandom() < 0.01)
		{
			%obj.setStatusEffect($SE_passiveSlot, "lethargic");
			%obj.schedule(50000, "cleanLethargic");
		}
	}
	%obj.traitSchedule = %obj.schedule(getMax(500, $Despair::Traits::Tick), traitSchedule, %obj);
}

function checkTraitConflicts(%list, %trait)
{
	if (%trait $= "" || %list $= "")
		return false;

	%c = -1;
	%conflicts[%c++] = "Extra Tough	Frail";
	%conflicts[%c++] = "Athletic	Sluggish";
	%conflicts[%c++] = "Investigative	Squeamish";
	%conflicts[%c++] = "Optimistic	Mood Swings	Melancholic	Apathetic";
	%conflicts[%c++] = "Loudmouth	Softspoken";
	%conflicts[%c++] = "Medium	Schizo";
	%conflicts[%c++] = "Heavy Sleeper	Paranoid	Narcoleptic";

	%v = -1;
	while(%v++ <= %c)
	{
		if (findField(%conflicts[%v], %trait) != -1)
		{
			%conflist = %conflicts[%v];
			break;
		}
	}

	for(%i = 0; %i < getFieldCount(%list); %i++)
	{
		%a = getField(%list, %i);
		if(%a $= %trait || findField(%conflist, %a) != -1)
			return true;
	}

	return false;
}

function loadSchizoList()
{
	%file = new FileObject();
	%fileName = $Despair::Path @ "data/schizoWhisper.txt";

	if (!%file.openForRead(%fileName))
	{
		error("ERROR: Failed to open '" @ %fileName @ "' for reading");
		%file.delete();
		return;
	}

	deleteVariables("$Despair::SchizoListItem_*");
	%max = -1;

	while (!%file.isEOF())
	{
		%line = %file.readLine();
		$Despair::SchizoListItem[%max++] = %line;
	}

	%file.close();
	%file.delete();

	$Despair::SchizoListMax = %max;
}

if (!$Despair::LoadedSchizoList)
{
	$Despair::LoadedSchizoList = true;
	loadSchizoList();
}

function getSchizoText()
{
	%text = $Despair::SchizoListItem[getRandom($Despair::SchizoListMax)];
	return %text;
}