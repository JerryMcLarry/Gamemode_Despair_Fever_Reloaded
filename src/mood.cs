$Despair::Mood::Tick = 60000; //1 minute

function Player::moodSchedule(%obj)
{
	cancel(%obj.moodSchedule);
	if(%obj.getState() $= "Dead")
		return;

	if (!$Despair::Mood::Enabled || %obj.character.trait["Apathetic"])
		return;

	if($despairTrial $= "")
	{
        if(%obj.mood > 0)
            %obj.addMood(-1);
        else if (%obj.mood < 0)
            %obj.addMood(1);

		if (!%obj.character.trait["Investigative"])
		{
			%ges = %obj.getEnvironmentStress();
			%stress = getWord(%ges, 0);
			%foundCorpse = getWord(%ges, 1);

			if (%stress == 1 && %obj.character.trait["Squeamish"])
				%text = "You are nervous of your surroundings.";
			else if (%stress == 2)
				%text = "You are unnerved by your surroundings.";
			else if (%stress == 3)
				%text = "You are horrified of your surroundings!";

			if (%text !$= "" && $Sim::Time - %obj.lastMoodChange > 30 && %obj.lastMoodText !$= %text)
				%obj.addMood(-%stress * (%obj.character.trait["Squeamish"] ? 3 : 2), %text, true);
		}
    }
    else
    {
        %obj.mood = "";
		return;
    }

    %state = getMoodName(%obj.mood);

    if(isObject(%obj.client))
    {
        %obj.client.vignette = $EnvGuiServer::VignetteMultiply SPC $EnvGuiServer::VignetteColor;

        if(%state $= "sad")
        {
            %obj.client.vignette = "0 0 0 0.7";
        }
        else if(%state $= "depressed")
        {
            %obj.client.vignette = "0 0 0 1";
        }

        commandToClient(%obj.client, 'SetVignette', true, %obj.client.vignette);
    }
	%obj.moodSchedule = %obj.schedule(getMax(500, $Despair::Mood::Tick), moodSchedule, %obj);
}

function Player::setMood(%obj, %int, %text, %textcheck)
{
	if (!$Despair::Mood::Enabled || %obj.character.trait["Apathetic"])
		return;

	if(%textcheck && %obj.lastMoodText $= %text)
		return;
	%moodMin = -15;
	%moodMax = 10;

	if (%obj.character.trait["Optimistic"])
		%moodMin = -7; //Can only be sad
	if (%obj.character.trait["Melancholic"])
		%moodMax = 1; //can't be happy, rip

    %obj.lastMoodChange = $Sim::Time;
    %obj.mood = mClamp(%int, -15, 10);
    if(isObject(%obj.client) && %text !$= "")
    {
        %obj.client.chatMessage((%int >= 0 ? "  \c2++" : "  \c0--") @ "MOOD\c5: " @ %text);
        %obj.lastMoodText = %text;
    }
}

function Player::addMood(%obj, %int, %text, %textcheck)
{
	if (!$Despair::Mood::Enabled || %obj.character.trait["Apathetic"])
		return;

	if (%obj.character.trait["Mood Swings"])
		%int *= 2; //Eat burger to feel overjoyed.
    if(isObject(%obj.client) && %text !$= "")
    {
        %obj.client.chatMessage((%int >= 0 ? "  \c2++" : "  \c0--") @ "\c6MOOD\c5: " @ %text);
        %obj.lastMoodText = %text;
		%text = "";
    }
    %obj.setMood(%obj.mood + %int, %text, %textcheck);
}

function getMoodSmiley(%mood)
{
	%smiley = ":|";
	switch$ (%mood)
	{
		case "overjoyed":
			%smiley = "\c2:D";
		case "happy":
			%smiley = "\c2:)";
		case "neutral":
			%smiley = "\c6:l";
		case "sad":
			%smiley = "\c1:(";
		case "depressed":
			%smiley = "\c7:(";
	}
	return %smiley;
}

function getMoodColor(%mood)
{
	%color = "\c0";
	switch$ (%mood)
	{
		case "overjoyed":
			%color = "\c2";
		case "happy":
			%color = "\c2";
		case "neutral":
			%color = "\c6";
		case "sad":
			%color = "\c1";
		case "depressed":
			%color = "\c7";
	}
	return %color;
}

function getMoodName(%int)
{
	%text = "\c0";
	if (%int >= 7)
		%text = "overjoyed";
    else if (%int >= 2)
		%text = "happy";
    else if (%int >= -2)
		%text = "neutral";
    else if (%int >= -7)
		%text = "sad";
    else
		%text = "depressed";

	return %text;
}