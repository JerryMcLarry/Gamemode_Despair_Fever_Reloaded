function despairBottomPrintLoop()
{
	cancel($bottomPrintSchedule);
	for (%i = 0; %i < $DefaultMiniGame.numMembers; %i++)
	{
		%client = $DefaultMiniGame.member[%i];
		%client.updateBottomprint();
	}
	$bottomPrintSchedule = schedule(1000, 0, despairBottomPrintLoop);
}

function GameConnection::despairCorpseVignette(%this, %ticks, %intensity)
{
	cancel(%this.despairCorpseVignette);
	if(!isObject(%this.player) || %this.player.health <= 0) //doesn't exist or is in crit (or dead)
		return;

	if(%ticks $= "")
		%ticks = 100;

	if(%ticks <= 0)
	{
		if(%this.vignette $= "")
			%this.vignette = $EnvGuiServer::VignetteColor SPC $EnvGuiServer::VignetteMultiply;
		commandToClient(%this, 'SetVignette', true, %this.vignette);
		return;
	}

	if(%ticks <= 10)
		%intensity = getMax(0, %intensity - 0.1);
	else if(%intensity < 0.9)
		%intensity += 0.05;
	else
		%intensity = 0.9 + getRandom() * 0.2;

	commandToClient(%this, 'SetVignette', true, mSin($Sim::Time) * 0.8 SPC "0 0" SPC getMin(1, %intensity));
	%this.despairCorpseVignette = %this.schedule(50, despairCorpseVignette, %ticks--, %intensity);
}

function GameConnection::updateBottomprint(%this)
{
	%client = %this;
	if (!isObject(%client.player) && isObject(%cam = %this.getControlObject()) && %cam.getClassName() $= "Camera" && isObject(%targ = %cam.getOrbitObject().client))
	{
		%client = %targ;
		%isSpectate = 1;
	}

	//CLOCK
	if(isEventPending($defaultMiniGame.eventSchedule))
	{
		%subtimer = getTimeString(mFloor(getTimeRemaining($defaultminigame.eventSchedule)/1000));
	}

	%timestr = %subtimer;
	if(!$despairTrial)
	{
		%time = getDayCycleTime();
		%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
		%time = %time - mFloor(%time); //get rid of excess stuff

		%time = getDayCycleTimeString(%time, 1);
		%mod12 = getWord(%time, 1);
		%time = getWord(%time, 0) SPC (%mod12 $= "PM" ? "<color:7e7eff>" : "<color:ffbf7e>") @ %mod12;

		%timestr = "D" @ $days @ "\c7|\c6" @ %time TAB %subtimer;
	}

	//NAME AND STUFF
	%name = %name @ "\c6" @ getCharacterName(%client.character, 1);

	if ((%mood = %client.player.mood) !$= "")
		%name = %name @ " " @ getMoodSmiley(getMoodName(%mood));

	//STATUS
	if(isObject(%player = %client.player))
	{
		for(%i=0; %i<$SE_maxStatusEffects; %i++)
		{
			%se = %player.statusEffect[%i];
			%color = getStatusEffectColor(%se);
			%stats = %stats SPC %color @ %se;
		}

		if(%player.IsBloody())
			%cosmetics = %cosmetics @ "\c0bloody";
		if(isObject(%player.itemProps[%player.weaponSlot]) && %player.itemProps[%player.weaponSlot].bloody)
			%cosmetics = %cosmetics SPC "\c0bloody wep";
		if(isObject(%hat = %player.tool[%player.hatSlot]) && %hat.disguise && isObject(%player.getMountedImage(2)) && %player.getMountedImage(2) == nameToID(%hat.image))
			%cosmetics = %cosmetics SPC "\c5disguised";
	}

	if(%isSpectate)
		%a = "\c3 (Spectating)";

	if(!%isSpectate || %this.showRoles)
	{
		if(!$pickedKiller)
			%role = "\c7Unknown";
		else if(%client.killer)
			%role = "\c0Killer";
		else
			%role = "\c2Innocent";
	}
	%str = "<font:Verdana:26><tab:150,240><color:FFFFFF>" @ %timestr @ "<just:right>" @ %stats @ "<just:left>\n" @ %name @ %a @ "<just:right>" @ %cosmetics @ "<just:left>\n\c6Role: " @ %role;

	commandToClient(%this, 'bottomPrint', %str, 0, 1);
}