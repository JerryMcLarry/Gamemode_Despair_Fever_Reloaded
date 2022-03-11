datablock itemData(DisguiseItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/satchel.dts";
	image = DisguiseImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.6 0.6 0.6 1";
	uiName = "Disguise";
	canDrop = true;

	itemPropsClass = "DisguiseProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = false;
};

function DisguiseProps::onAdd(%this)
{
	%this.kira = false;
}

datablock ShapeBaseImageData(DisguiseImage)
{
	item = DisguiseItem;
	shapeFile = "base/data/shapes/empty.dts";
	armReady = false;

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

function DisguiseImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps(%obj.currTool);
	fixArmReady(%obj);
	if (isObject(%obj.client) && %obj.client.killer)
		commandToClient(%obj.client, 'CenterPrint',
		"<color:FFFF00>Click on a corpse to completely steal their identity."
		@ (%props.kira ? "\nThis is the \c0KIRA VERSION\c3!!! Infinite use and the body disappears!" : "\nThis is one use and will mangle the corpse beyond recognition.") @
		"\n\c0WARNING\c6: Be sure to switch your keys with the victim!");
}

function DisguiseImage::onUnMount(%this, %obj, %slot)
{
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function DisguiseImage::onUse(%this, %obj, %slot)
{
	%props = %obj.getItemProps(%obj.currTool);
	if(!isObject(%obj.client) || !%obj.client.killer)
		return;
	if(isObject(%col = %obj.findCorpseRayCast()))
	{
		if(%col.isDead)
		{
			%obj.character.appearance = %col.character.appearance;
			%obj.fakeName = %col.character.name;
			%obj.character.gender = %col.character.gender;
			%col.mangled = true;
			%col.character.mangled = true;
			%obj.applyAppearance();
			%col.applyAppearance();

			if(%obj.character.trait["Softspoken"])
			{
				%obj.character.trait["Softspoken"] = false; //Can potentially give you away
			}
			if (isObject(%obj.client))
			{
				commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>You take on the appearance of\c6" SPC %col.character.name, 2);
				RS_Log(%obj.client.getPlayerName() SPC "(" @ %obj.client.getBLID() @ ") used a disguise on" SPC %col.character.name @ "!", "\c2");
			}
			if (%props.kira)
			{
				for(%i=0;%i<%col.getDataBlock().maxTools;%i++)
				{
					%col.dropTool(%i);
				}
				%col.delete();
			}
			else
				%obj.removeTool(%obj.currTool);
		}
		else
		{
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Target must be dead for this to work!", 2);
		}
	}
}