$Despair::radioNumChannels = 1;

datablock AudioProfile(radioGetSound)
{
	filename = $Despair::Path @ "res/sounds/radioGet.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(radioLoseSound)
{
	filename = $Despair::Path @ "res/sounds/radioLose.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(radioJoinSound)
{
	filename = $Despair::Path @ "res/sounds/radioJoin.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(radioTalkSound)
{
	filename = $Despair::Path @ "res/sounds/radioTalk.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock ItemData(RadioItem)
{
	image = RadioImage;
	shapeFile = $Despair::Path @ "res/shapes/items/Radio.dts";

	uiName = "Radio";
	canDrop = true;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";

	image = RadioImage;

	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	emap = true;

	itemPropsClass = "RadioProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = false;
};

function RadioProps::onAdd(%this)
{
	%this.channel = 0;
}

datablock ShapeBaseImageData(RadioImage)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Radio.dts";
	hasLight = true;

	item = RadioItem;

	emap = true;
	offset = "0 0 0";
	mountPoint = 0;
	armReady = false;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.5;
	stateTransitionOnTimeout[0]		= "Ready";
	stateSound[0]					= "";
	
	stateName[1] = "Ready";
	stateAllowImageChange[1] = true;
	stateTransitionOnTriggerDown[1] = "Use";

	stateName[2] = "Use";
	stateScript[2] = "onUse";
	stateAllowImageChange[2] = false;
	stateTransitionOnTriggerUp[2] = "Ready";
};

function RadioItem::onPickup(%this, %obj, %player, %slot)
{
	%props = %player.getItemProps(%slot);
	serverPlay3d("radioGetSound", %player.getHackPosition());
	if(isObject(%player.client))
		%player.client.centerPrint("\c5Connected to channel\c3 "@ (%props.channel+1) @ "\n\c5Click to change channels.", 3);
	radioJoined(%player, %props.channel);
}

function RadioItem::onDrop(%this, %player, %slot)
{
	%props = %player.getItemProps(%slot);
	serverPlay3d("radioLoseSound", %player.getHackPosition());
	
	if(isObject(%player.client))
	{
		messageClient(%player.client, 'MsgItemPickup', '', %slot, "");
		%player.client.centerPrint("\c5Radio disconnected.", 3);
	}
	%player.tool[%slot] = "";
	radioLeft(%player, %props.channel);
}
function RadioImage::onUse(%this, %obj, %slot)
{
	if ($Despair::radioNumChannels <= 1)
		return;
	%props = %obj.getItemProps();
	radioLeft(%obj, %props.channel);
	%props.channel = (%props.channel + 1) % $Despair::radioNumChannels;
	if(isObject(%obj.client))
		%obj.client.centerPrint("\c5Connected to channel\c3 "@ (%props.channel+1), 3);
	radioJoined(%obj, %props.channel);
	serverPlay3d("radioGetSound", %obj.getHackPosition());
}

function RadioImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		%obj.client.centerPrint("\c5Connected to channel\c3 " @ (%props.channel + 1));
}
function RadioImage::onUnMount(%this, %obj, %slot)
{
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function radioJoined(%obj, %channel)
{
	%time = getDayCycleTime();
	%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
	%time = %time - mFloor(%time); //get rid of excess stuff

	%time = getDayCycleTimeString(%time, 1);

	%name = "[" @ (%obj % 100) @ "] Someone";

	if(isObject(%obj.client))
		RS_Log(%obj.client.getPlayerName() SPC "(" @ %obj.client.getBLID() @ ") joined radio channel '" @ %channel @ "'", "\c2");
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%member = ClientGroup.getObject(%i);
		if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
			continue;

		if((%slot = %member.player.findTool("RadioItem")) != -1)
		{
			%props = %member.player.getItemProps(%slot);
			if(%props.channel != %channel)
				continue;
		}
		else
			continue;

		if(%member.player == %obj)
			continue;

		serverPlay3d("radioJoinSound", %member.player.getHackPosition());
		if(%member.player.unconscious)
			continue;

		messageClient(%member, '', '\c7[%1]<color:62f069>[Ch.#%2] %3 has joined the channel.', %time, %channel+1, %name);
	}
}

function radioLeft(%obj, %channel)
{
	%time = getDayCycleTime();
	%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
	%time = %time - mFloor(%time); //get rid of excess stuff

	%time = getDayCycleTimeString(%time, 1);

	%name = "[" @ (%obj % 100) @ "] Someone";

	if(isObject(%obj.client))
		RS_Log(%obj.client.getPlayerName() SPC "(" @ %obj.client.getBLID() @ ") left radio channel '" @ %channel @ "'", "\c2");
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%member = ClientGroup.getObject(%i);
		if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
			continue;

		if((%slot = %member.player.findTool("RadioItem")) != -1)
		{
			%props = %member.player.getItemProps(%slot);
			if(%props.channel != %channel)
				continue;
		}
		else
			continue;

		if(%member.player == %obj)
			continue;

		serverPlay3d("radioLoseSound", %member.player.getHackPosition());
		if(%member.player.unconscious)
			continue;

		messageClient(%member, '', '\c7[%1]<color:62f069>[Ch.#%2] %3 has left the channel.', %time, %channel+1, %name);
	}
}

function radioTransmitMessage(%obj, %channel, %text)
{
	%time = getDayCycleTime();
	%time += 0.25; //so Zero = 6 AM aka morning, Youse's daycycle begins from morning at 0 fraction
	%time = %time - mFloor(%time); //get rid of excess stuff

	%time = getDayCycleTimeString(%time, 1);

	%name = "(" @ (%obj % 100) @ ")Someone";
	%text = scrambleText(%text, 0.1);
	for (%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%member = ClientGroup.getObject(%i);
		if (!isObject(%member.player) || %member.miniGame != $DefaultMiniGame)
			continue;

		if((%slot = %member.player.findTool("RadioItem")) != -1)
		{
			%props = %member.player.getItemProps(%slot);
			if(%props.channel != %channel)
				continue;
		}
		else
			continue;

		serverPlay3d("radioTalkSound", %member.player.getHackPosition());

		if(%member.player.unconscious)
			continue;

		messageClient(%member, '', '\c7[%1]<color:62f069>[Ch.#%2] %3 radios<color:99ffa0>, %4', %time, %channel+1, %name, %text);
	}
}