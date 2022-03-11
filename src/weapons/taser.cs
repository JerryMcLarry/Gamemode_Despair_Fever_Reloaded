datablock AudioProfile(TaserHitSound)
{
	fileName = $Despair::Path @ "res/sounds/weapons/taser.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock ParticleEmitterData(TaserEmitter : radioWaveTrailEmitter)
{
	useEmitterColors = false;
};

datablock itemData(TaserItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Taser.dts";
	image = TaserImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.4 0.4 0.6 1";
	uiName = "Taser";
	canDrop = true;
	itemPropsClass = "TaserProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = false;
};

function TaserProps::onAdd(%this)
{
	%this.spent = false;
}

datablock ShapeBaseImageData(TaserImage)
{
	item = TaserItem;
	shapeFile = $Despair::Path @ "res/shapes/items/Taser.dts";
	doColorShift = true;
	colorShiftColor = "0.4 0.4 0.6 1";
	offset = "0 0 0";
	rotation = eulerToMatrix("0 0 0");

	armReady = false;

	stateName[0]					= "Activate";
	stateTimeoutValue[0]			= 0.2;
	stateTransitionOnTimeout[0]		= "Ready";
	stateSound[0]					= "";
	
	stateName[1] = "Ready";
	stateAllowImageChange[1] = true;
	stateTransitionOnTriggerDown[1] = "Use";
	stateSequence[1] = "root";
	stateScript[1] = "onReady";
	stateTransitionOnNoAmmo[1] = "Empty";

	stateName[2] = "Use";
	stateScript[2] = "onUse";
	stateAllowImageChange[2] = false;
	stateTimeoutValue[2] = 0.1;
	stateWaitForTimeout[2] = true;
	stateTransitionOnTimeOut[2] = "Wait";
	stateSequence[2] = "fire";
	stateEmitterTime[2] = 0.2;
	stateEmitter[2] = "TaserEmitter";
	stateTransitionOnNoAmmo[2] = "Empty";

	stateName[3] = "Wait";
	stateWaitForTimeout[3] = true;
	stateAllowImageChange[3] = false;
	stateTimeoutValue[3] = 0.7;
	stateTransitionOnTimeout[3] = "Ready";

	stateName[4] = "Empty";
	stateWaitForTimeout[4] = false;
	stateAllowImageChange[4] = true;
	stateTransitionOnAmmo[4] = "Ready";
};

function TaserImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	%props = %obj.getItemProps();
	if (%props.spent > 0)
		%obj.setImageAmmo(0, 0);
	else
		%obj.setImageAmmo(0, 1);
}

function TaserImage::onUnMount(%this, %obj, %slot)
{
}

function TaserImage::onReady(%this, %obj, %slot)
{
	%obj.playThread(1, "root");
}

function TaserImage::onUse(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (%props.spent > 0)
	{
		%obj.setImageAmmo(0, 0);
		return;
	}
	else
		%obj.setImageAmmo(0, 1);

	%obj.playThread(1, "armReadyRight");

	%a = %obj.getEyePoint();
	%b = vectorAdd(%a, vectorScale(%obj.getEyeVector(), 5));

	%mask =
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::PlayerObjectType;

	%ray = containerRayCast(%a, %b, %mask, %obj);
	if(%ray && %ray.getType() & $TypeMasks::PlayerObjectType)
	{
		%props.spent = true;
		serverPlay3d(TaserHitSound, getWords(%ray, 1, 3));
		%ray.KnockOut($Despair::SleepKnockout);
	}
	else
	{
		serverPlay3d(radioWaveExplosionSound, %obj.getHackPosition());
	}
}