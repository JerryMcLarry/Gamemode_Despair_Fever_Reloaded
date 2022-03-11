
// Sounds
datablock audioProfile(SpraySound)
{
	fileName =  $Despair::Path @ "res/sounds/spray.wav";
	description = AudioQuiet3d;
	preload = true;
};
datablock audioProfile(SprayEmptySound)
{
	fileName =  $Despair::Path @ "res/sounds/sprayEmpty.wav";
	description = AudioQuiet3d;
	preload = true;
};

// Standard
datablock ParticleData(CleanSprayTrailParticle : blinkCleanSprayParticle)
{
	textureName		= $Despair::Path @ "res/shapes/items/blur";

	dragCoefficient			= 3.0;
	windCoefficient			= 0.5;
	gravityCoefficient		= 0.1;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS				= 420;
	lifetimeVarianceMS		= 0;
	spinSpeed				= 0.0;
	spinRandomMin			= -50.0;
	spinRandomMax			= 50.0;
	useInvAlpha				= false;
	animateTexture			= false;

	// Interpolation variables
	colors[0]	= "0.8 0.8 1 1";
	colors[1]	= "0.5 0.5 0.8 1";
	colors[2]	= "0.2 0.2 0.8 0.4";

	sizes[0]	= "0";
	sizes[1]	= "0.4";
	sizes[2]	= "0.3";
	sizes[3]	= "0.1";

	times[0]	= "0";
	times[1]	= "0.5";
	times[2]	= "0.9";
	times[3]	= "1";
};

datablock ParticleEmitterData(CleanSprayTrailEmitter)
{
   ejectionPeriodMS = 2;
   periodVarianceMS = 0;

   ejectionVelocity = 40;
   velocityVariance = 5;

   ejectionOffset = 0;

   thetaMin         = 0;
   thetaMax         = 6;  

   phiReferenceVel = 0;
   phiVariance = 360;

   particles = CleanSprayTrailParticle;
};

datablock ItemData(CleanSprayItem)
{
	image = CleanSprayImage;
	shapeFile = $Despair::Path @ "res/shapes/items/cleanspray.dts";

	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;

	doColorShift = true;
	colorShiftColor = "0.2 0.5 0.75 1";

	uiName = "Clean Spray";
	canDrop = true;

	itemPropsClass = "CleanerProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = false;

	waitForKiller = true; //Wait for killer to be picked before this can be picked up
};

function CleanerProps::onAdd(%this)
{
	%this.ammo = getRandom(40, 60);
}
datablock ShapeBaseImageData(CleanSprayImage)
{
	shapeFile = $Despair::Path @ "res/shapes/items/cleanspray.dts";
	emap = false;

	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0.7 1.2 -0.55";
	eyeRotation = "1 0 0 0";
	rotation = "1 0 0 0";

	doColorShift = true;
	colorShiftColor = "0.2 0.5 0.75 1";

	correctMuzzleVector = true;

	className = "WeaponImage";

	item = CleanSprayItem;
	ammo = " ";
	projectile = "";
	projectileType = projectile;
	melee = false;
	armReady = true;

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.4;
	stateWaitForTimeout[0] = false;
	stateAllowImageChange[0] = true;
	stateTransitionOnTimeout[0] = "Ready";
	stateSequence[0] = "root";
	//stateSound[0] = "sprayActivateSound";

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1] = true;
	stateTransitionOnNoAmmo[1] = "Empty";

	stateName[2] = "Fire";
	stateFire[2] = true;
	stateScript[2] = "onFire";
	stateTimeoutValue[2] = 0.3;
	stateWaitForTimeout[2] = true;
	stateAllowImageChange[2] = false;
	stateTransitionOnTriggerUp[2] = "Ready";
	stateSequence[2] = "fire";
	stateSound[2] = "";
	stateEmitterTime[2] = 0.06;
	stateEmitter[2] = "CleanSprayTrailEmitter";
	stateTransitionOnNoAmmo[2] = "Empty";

	stateName[3] = "Empty";
	stateWaitForTimeout[3] = false;
	stateAllowImageChange[3] = true;
	stateTransitionOnAmmo[3] = "Ready";
	stateTransitionOnTriggerDown[3] = "EmptyFire";

	stateName[4] = "EmptyFire";
	stateScript[4] = "onEmptyFire";
	stateTimeoutValue[4] = 0.3;
	stateWaitForTimeout[4] = true;
	stateAllowImageChange[4] = false;
	stateTransitionOnTriggerUp[4] = "Empty";
	stateSequence[4] = "fire";
};

function CleanSprayImage::onEmptyFire(%this, %obj, %slot)
{
	serverPlay3d("SprayEmptySound", %obj.getHackPosition());
	%props = %obj.getItemProps();
	if(getRandom() <= 0.1)
		%props.ammo++;
	if (%props.ammo > 0)
		%obj.setImageAmmo(0, 1);
}

function CleanSprayImage::onFire(%this, %obj, %slot)
{
	if($investigationStart !$= "" && !%obj.client.killer)
		return;
	%props = %obj.getItemProps();
	if (%props.ammo <= 0)
	{
		%obj.setImageAmmo(0, 0);
		return;
	}
	else
		%obj.setImageAmmo(0, 1);

	serverPlay3d("SpraySound", %obj.getHackPosition());

	%point = %obj.getEyePoint();
	%vector = %obj.getEyeVector();
	%stop = vectorAdd(%point, vectorScale(%vector, 7));

	%ray = containerRayCast(%point, %stop,
		$TypeMasks::FxBrickObjectType |
		$TypeMasks::StaticShapeObjectType |
		$TypeMasks::TerrainObjectType |
		$TypeMasks::playerObjectType |
		$TypeMasks::ItemObjectType,
		%obj
	);

	if (isObject(firstWord(%ray))) {
		%pos = getWords( %ray, 1, 3 );

		if (%ray.getType() & $TypeMasks::playerObjectType)
			%ray.setWhiteOut(0.1);
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

		%clean = getMin(%col.freshness, 0.8);
		if (%col.freshness <= 0)
		{
			%col.delete();
			continue;
		}
		if (%clean <= 0)
			continue;
		if (%props.ammo <= 0)
		 	return;
		%col.freshness = getMin(%col.freshness - %clean, 0.9);
		%col.color = getWords(%col.color, 0, 2) SPC getWord(%col.color, 3) * 0.5;
		%col.setNodeColor("ALL", %col.color);
		%col.setScale(vectorScale(%col.getScale(), 0.8));
		%props.ammo--;
	}

	//if(%clean)
	//	RS_Log(%client.getPlayerName() SPC "(" @ %client.getBLID() @ ") cleaned something with the clean spray!", "\c1");
}