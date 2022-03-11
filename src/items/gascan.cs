//todo: make this not a push broom reskin lol

datablock ItemData(gasCanItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/gascan.dts";
	emap = true;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "Gas Can";
	iconName = "";
	doColorShift = true;
	colorShiftColor = "0.4 0.2 0";

	image = gasCanImage;

	canDrop = true;

	waitForKiller = true; //Wait for killer to be picked before this can be picked up
};

function gasCanItem::onPickup(%this, %obj, %player, %i)
{
	%player.gasCanSlot = %i;
	%player.currtool = %i;
	serverCmdUseTool(%player.client, %i);
	commandToClient(%player.client, 'SetActiveTool', %i);
}

datablock ShapeBaseImageData(gasCanImage)
{
	shapeFile = $Despair::Path @ "res/shapes/items/gascan.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0.4";
	correctMuzzleVector = false;

	rotation = eulerToMatrix("180 0 0");

	className = "WeaponImage";

	item = gasCanItem;

	melee = true;
	armReady = true;

	doColorShift = true;
	colorShiftColor = gasCanItem.colorShiftColor;

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.0;
	stateTransitionOnTimeout[0]		= "Ready";

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "PreFire";
	stateAllowImageChange[1]		= true;

	stateName[2]					= "PreFire";
	stateScript[2]					= "onPreFire";
	stateAllowImageChange[2]		= true;
	stateTimeoutValue[2]			= 0.01;
	stateTransitionOnTimeout[2]		= "Fire";

	stateName[3]					= "Fire";
	stateTransitionOnTimeout[3]		= "Fire";
	stateTimeoutValue[3]			= 0.2;
	stateFire[3]					= true;
	stateAllowImageChange[3]		= true;
	stateSequence[3]				= "Fire";
	stateScript[3]					= "onFire";
	stateWaitForTimeout[3]			= true;
	stateSequence[3]				= "Fire";
	stateTransitionOnTriggerUp[3]	= "StopFire";
	stateSound[3]					= "";
	//stateTransitionOnTriggerUp[3]	= "StopFire";

	stateName[4]					= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "Fire";
	stateSound[4]					= "";

	stateName[5]					= "StopFire";
	stateTransitionOnTimeout[5]		= "Ready";
	stateTimeoutValue[5]			= 0.2;
	stateAllowImageChange[5]		= true;
	stateWaitForTimeout[5]			= true;
	stateSequence[5]				= "StopFire";
	stateScript[5]					= "onStopFire";
};

function gasCanImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, shiftAway);
	if($investigationStart !$= "")
		return;
	%point = %obj.getEyePoint();
	%vector = %obj.getEyeVector();
	%stop = vectorAdd(%point, vectorScale(%vector, 7));

	%ray = containerRayCast(%point, %stop,
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::StaticShapeObjectType |
		$TypeMasks::TerrainObjectType |
		$TypeMasks::ItemObjectType,
		%obj
	);

	if (isObject(firstWord(%ray))) {
		%pos = getWords( %ray, 1, 3 );
	}
	else {
		%pos = %stop;
	}

	initContainerRadiusSearch(%pos, 0.75,
		$TypeMasks::StaticShapeObjectType);

	while (isObject(%col = containerSearchNext()))
	{
		if (!%col.getDataBlock().canClean)
			continue;

		%clean = getMin(%col.freshness, 0.5);
		if (%col.freshness <= 0)
		{
			%col.delete();
			continue;
		}
		if (%clean <= 0)
			continue;
		%col.freshness = getMin(%col.freshness - %clean, 0.9);
		%col.color = getWords(%col.color, 0, 2) SPC getWord(%col.color, 3) * 0.5;
		%col.setNodeColor("ALL", %col.color);
		%col.setScale(vectorScale(%col.getScale(), 0.8));
	}

	//if(%clean)
	//	RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") cleaned something with the gasCan!", "\c1");
}

function gasCanImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, root);
}

function gasCanImage::onUnMount(%this, %obj, %slot)
{
	if(%obj.tool[%obj.gasCanSlot] !$= "")
		%obj.dropTool(%obj.gasCanSlot);
}