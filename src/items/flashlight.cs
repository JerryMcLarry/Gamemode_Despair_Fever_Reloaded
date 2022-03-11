$Despair::FlashlightRange = 8;
$Despair::FlashlightRate = 50;
$Despair::FlashlightSpeed = 0.54;
$Despair::FlashlightVelIntFactor = 0.2;
datablock AudioProfile(FlashlightToggleSound)
{
	fileName = $Despair::Path @ "res/sounds/toggle_flashlight.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock FxLightData(PlayerFlashlight : PlayerLight)
{
	uiName = "";
	flareOn = 0;

	radius = 16;
	brightness = 3;
};

datablock ItemData(FlashlightItem)
{
	image = FlashlightImage;
	shapeFile = $Despair::Path @ "res/shapes/items/flashlight.dts";

	uiName = "Flashlight";
	canDrop = true;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";

	image = FlashLightImage;

	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	emap = true;
};

datablock ShapeBaseImageData(FlashlightImage)
{
	shapeFile = $Despair::Path @ "res/shapes/items/flashlight.dts";
	hasLight = true;

	item = FlashlightItem;

	emap = true;
	offset = "0 0 0";
	mountPoint = 0;
	armReady = false;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.01;
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

//datablock ShapeBaseImageData(FlashlightMountedImage : FlashLightImage)
//{
//	mountPoint = 1;
//};
function flashLightImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	//if (isObject(%obj.getMountedImage(1)) && isObject(%obj.light))
	//{
	//	%obj.unMountImage(1);
	//}
}

function flashLightImage::onUnMount(%this, %obj, %slot)
{
	if (isObject(%obj.light))
	{
		serverPlay3D(FlashlightToggleSound, %obj.getHackPosition());
		%obj.light.delete();
	}
	//if (!isObject(%obj.getMountedImage(1)) && isObject(%obj.light))
	//{
	//	%obj.mountImage("flashLightMountedImage", 1);
	//}
}

function flashLightImage::onUse(%this, %obj, %slot){
	if (!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	if (getSimTime() - %obj.lastLightTime < 250)
	{
		return;
	}

	%obj.lastLightTime = getSimTime();
	serverPlay3D(FlashlightToggleSound, %obj.getHackPosition());

	if (isObject(%obj.light))
	{
		%obj.light.delete();

		//if (%obj.getMountedImage(1) == nameToID("FlashlightMountedImage"))
		//{
		//	%obj.unMountImage(1);
		//}
	}
	else
	{
		%obj.light = new FxLight()
		{
			datablock = playerFlashlight;
			obj = %obj;

			iconSize = 1;
			enable = 1;
		};

		missionCleanup.add(%obj.light);
		%obj.light.setTransform(%obj.getTransform());

		if (!isEventPending(%obj.flashlightTick))
		{
			%obj.flashlightTick();
		}
	}
}

function Player::flashlightTick(%this, %slot)
{
	cancel(%this.flashlightTick);

	if (%this.getState() $= "Dead" || !isObject(%this.light))
	{
		return;
	}

	%mask = 0;

	%mask |= $TypeMasks::StaticShapeObjectType;
	%mask |= $TypeMasks::FxBrickObjectType;
	%mask |= $TypeMasks::VehicleObjectType;
	%mask |= $TypeMasks::TerrainObjectType;
	%mask |= $TypeMasks::PlayerObjectType;

	%range = $Despair::FlashlightRange;

	if (%range $= "" && $EnvGuiServer::VisibleDistance !$= "")
	{
		%limit = $EnvGuiServer::VisibleDistance / 2;

		if (%range > %limit)
		{
			%range = %limit;
		}
	}

	%slot = %this.getMountedImage(0) == nameToID("FlashlightImage") ? 0 : 1;

	%start = %this.getMuzzlePoint(%slot);
	%losPoint = %this.getLOSPoint(%this.getEyePoint(), %this.getAimVector(), %mask, %range);

	//%vector = %this.getAimVector();
	%vector = vectorNormalize(vectorSub(%losPoint, %start));

	%end = vectorAdd(%start, vectorScale(%vector, %range));
	%end = vectorAdd(%end, vectorScale(%this.getVelocity(), $Despair::FlashlightVelIntFactor));

	%ray = containerRayCast(%start, %end, %mask, %this);

	if (!%ray)
	{
		%pos = %end;
	}
	else
	{
		%pos = vectorSub(getWords(%ray, 1, 3), %vector);
	}

	%path = vectorSub(%pos, %this.light.position);
	%length = vectorLen(%path);

	if (%length < $Despair::FlashlightSpeed)
	{
		%pos = %pos;
	}
	else
	{
		%moved = vectorScale(%path, $Despair::FlashlightSpeed);
		%pos = vectorAdd(%this.light.position, %moved);
	}

	%this.light.setTransform(%pos);
	%this.light.reset();

	%this.flashlightTick = %this.schedule($Despair::FlashlightRate, "flashlightTick", %slot);
}