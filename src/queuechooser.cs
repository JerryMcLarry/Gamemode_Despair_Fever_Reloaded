function chooseNextClient(%queueName)
{
	%queue = $Despair::Queue[%queueName];
	while(!isObject(%pick) && !%refreshed)
	{
		%index = getRandom(0, getWordCount(%queue)-1);
		%pick = getWord(%queue, %index);
		%queue = removeWord(%queue, %index);
		if(!isObject(%pick) || !isObject(%pl = %pick.player) || %pick.miniGame != $defaultMiniGame || %pl.noWeapons || %cl.afk)
			%pick = "";
		else
			break;
		if(getWordCount(%queue) <= 0)
		{
			%j = -1;
			for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{
				%cl = ClientGroup.getObject(%i);
				if (!isObject(%pl = %cl.player) || %cl.miniGame != $defaultMiniGame || %pl.noWeapons || %cl.afk)
					continue;
				
				%queue = setWord(%queue, %j++, %cl);
			}
			%refreshed = true;
		}
		%index = getRandom(0, getWordCount(%queue)-1);
		%pick = getWord(%queue, %index);
		%queue = removeWord(%queue, %index);
		if(!isObject(%pick) || !isObject(%pl = %pick.player) || %pick.miniGame != $defaultMiniGame || %pl.noWeapons || %cl.afk)
			%pick = "";
	}
	if(!isObject(%pick) && %refreshed)
	{
		talk(announce("\c0ERROR\c6: No suitable clients found to be killer. The fuck!?"));
		return -1;
	}
	$Despair::Queue[%queueName] = %queue;
	return %pick;
}