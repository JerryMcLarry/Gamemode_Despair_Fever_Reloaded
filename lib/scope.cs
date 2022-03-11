function scopeToAll(%object)
{
	%i = ClientGroup.getCount();

	while (%i-- >= 0)
	{
		%client = ClientGroup.getObject(%i);

		if (%client.hasSpawnedOnce)
			%object.scopeToClient(%client);
	}
}