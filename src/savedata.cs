function UpdatePeopleScore()
{
	for(%i=0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		if(!$DefaultMiniGame.permaDeath && $DefaultMiniGame.numMembers < 6)
		{
			messageClient(%client, '', '\c2>>There must be at least 6 players for the score system.');
			return;
		}
		%client.points += %client.TempPoints;
		if (%client.points > %client.pointstotal)
			%client.pointstotal = %client.points;
		%client.setScore();
		%client.dfSaveData();
		messageClient(%client, '', '\c2>>Your total score for the round is %2%1 points\c2!', %client.TempPoints, %client.TempPoints > 0 ? "\c6" : "\c0");
		%client.TempPoints = 0;
	}
}

function GameConnection::AddPoints(%client, %num)
{
	if(%num $= "" || %num == 0)
		return;
	if($DefaultMiniGame.numMembers < 6)
		return;
	messageClient(%client, '', '\c2>>You recieved %2%1 points\c2!', %num, %num > 0 ? "\c6" : "\c0");
	if($DefaultMiniGame.permaDeath && %num > 0)
	{
		%bonus = %num; //double the points
		messageClient(%client, '', '\c2>>Due to active \c0PERMADEATH\c2, you recieved \c6%1 points bonus\c2!', %bonus);
		%num += %bonus;
	}
	%client.TempPoints += %num;
}

function GameConnection::SpendPoints(%client, %num)
{
	if(%num $= "" || %num == 0)
		return false;
	if($DefaultMiniGame.numMembers < 6)
		return false;
	if(%client.points >= %num)
	{
		%client.points -= %num;
		%client.setScore();
		%client.dfSaveData();
		return true;
	}
	return false;
}

function gameConnection::dfSaveData(%this)
{
	%name = "config/server/despairfever/data/" @ %this.getBLID() @ ".txt";

	%file = new FileObject();
	%file.openForWrite(%name);

	if(!%file)
	{
		error("ERROR: GameConnection::dfSaveData(" @ %this @ " (BLID: " @ %this.getBLID() @ ")) - failed to open file '" @ %name @ "' for write");

		%file.delete();
		return;
	}

	echo("Saving data for BLID " @ %this.getBLID());

	%file.writeLine("//data for " @ %this.getPlayerName() @ ", generated at " @ getDateTime());

	%file.writeLine("score" TAB %this.points);
	%file.writeLine("score total" TAB %this.pointstotal);
	%file.writeLine("killer wins" TAB %this.killerWins);
	%file.writeLine("murders" TAB %this.murders);
	%file.writeLine("innocent wins" TAB %this.innocentWins);
	%file.writeLine("correct votes" TAB %this.correctVotes);
	%file.writeLine("deaths" TAB %this.deaths);
	%file.writeLine("killerbanned" TAB %this.killerbanned);

	%file.close();
	%file.delete();
}

function GameConnection::dfLoadData(%this)
{
	%name = "config/server/despairfever/data/" @ %this.getBLID() @ ".txt";

	%file = new FileObject();
	%file.openForRead(%name);

	if(!%file)
	{
		error("ERROR: GameConnection::dfLoadData(" @ %this @ " (BLID: " @ %this.getBLID() @ ")) - failed to open file '" @ %name @ "' for read");

		%file.delete();
		return;
	}

	echo("Loading data for BLID " @ %this.getBLID());

	while(!%file.isEOF())
	{
		%line = %file.readLine();

		%var = getField(%line, 0);
		%val = getFields(%line, 1, getFieldCount(%line));
		echo(%var SPC %val);
		if(strpos(%var, "//") == 0 || %val $= "")
		{
			continue;
		}

		switch$(%var)
		{
			case "score":
				%this.points = %val;
				%this.setScore();
			case "score total":
				%this.pointstotal = %val;
			case "killer wins":
				%this.killerWins = %val;
			case "murders":
				%this.murders = %val;
			case "innocent wins":
				%this.innocentWins = %val;
			case "correct votes":
				%this.correctVotes = %val;
			case "deaths":
				%this.deaths = %val;
			case "killerbanned":
				%this.killerbanned = %val;
		}
	}

	%file.close();
	%file.delete();

	//%this.SaveLoop(true);
}
