datablock AudioProfile(RazorStartSound)
{
	fileName = $Despair::Path @ "res/sounds/razorStart.wav";
	description = AudioClosest3d;
	preload = true;
};
datablock AudioProfile(RazorHitSound)
{
	fileName = $Despair::Path @ "res/sounds/razor.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock ParticleData(RazorParticle)
 {
   dragCoefficient      = 3;
   gravityCoefficient   = -0.0;
   inheritedVelFactor   = 0.15;
   constantAcceleration = 0.0;
   lifetimeMS           = 200;
   lifetimeVarianceMS   = 0;
   textureName          = "base/data/particles/dot";
   spinSpeed		   = 0.0;
   spinRandomMin		= 0.0;
   spinRandomMax		= 0.0;
   colors[0]     = "0.5 0.5 0.9 0.2";
   colors[1]     = "0.5 0.5 0.9 0.1";
   colors[2]     = "0.5 0.5 0.9 0.0";

   sizes[0]      = 0.18;
   sizes[1]      = 0.1;
   sizes[2]      = 0.0;

   times[0] = 0.0;
   times[1] = 0.5;
   times[2] = 1.0;

   useInvAlpha = false;
};
datablock ParticleEmitterData(RazorEmitter)
{
   ejectionPeriodMS = 15;
   periodVarianceMS = 1;
   ejectionVelocity = 0.25;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "RazorParticle";
};

datablock itemData(RazorItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/Razor.dts";
	image = RazorImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.7 0.7 0.7 1";
	uiName = "Razor";
	canDrop = true;
	itemPropsClass = "RazorProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = false;

	waitForKiller = true; //Wait for killer to be picked before this can be picked up
};

function RazorProps::onAdd(%this)
{
	%this.uses = getRandom(3, 6);
}

datablock ShapeBaseImageData(RazorImage)
{
	item = RazorItem;
	shapeFile = $Despair::Path @ "res/shapes/items/Razor.dts";
	doColorShift = true;
	colorShiftColor = "0.7 0.7 0.7 1";
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
	//stateSequence[1] = "root";
	stateScript[1] = "onReady";
	stateTransitionOnNoAmmo[1] = "Empty";

	stateName[2] = "Use";
	stateScript[2] = "onUse";
	stateAllowImageChange[2] = false;
	stateTimeoutValue[2] = 0.1;
	stateWaitForTimeout[2] = true;
	stateTransitionOnTimeOut[2] = "Wait";
	//stateSequence[2] = "fire";
	stateEmitterTime[2] = 0.2;
	stateEmitter[2] = "RazorEmitter";
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

function RazorImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	%props = %obj.getItemProps();
	if (%props.uses < 0)
	{
		%obj.setImageAmmo(0, 0);
		if (isObject(%obj.client))
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>It's dull...");
		return;
	}
	%obj.setImageAmmo(0, 1);
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Left Click to shave someone, Right Click to shave yourself");
}

function RazorImage::onUnMount(%this, %obj, %slot)
{
	cancel(%this.razeSchedule);
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}

function RazorImage::onReady(%this, %obj, %slot)
{
	%obj.playThread(1, "root");
}

function RazorImage::onUse(%this, %obj, %slot)
{
	if($InvestigationStart !$= "" && !%obj.client.killer)
		return;
	%props = %obj.getItemProps();
	if (%props.uses < 0)
	{
		%obj.setImageAmmo(0, 0);
		if (isObject(%obj.client))
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>It's dull...");
		return;
	}
	else
		%obj.setImageAmmo(0, 1);

	%obj.playThread(1, "armReadyRight");

	if(isObject(%col = %obj.findCorpseRayCast(1)))
	{
		if(getField(%col.character.appearance, 3) $= "")
		{
			if (isObject(%obj.client))
				commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>They're already bald!");
			return;
		}
		if(isObject(%img = %col.getMountedImage(2)))
		{
			if (isObject(%obj.client))
				commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Take off their hat first!");
			return;
		}
		serverPlay3d(RazorStartSound, %col.getHackPosition());
		%obj.setImageAmmo(0, 0);
		%obj.prevTargetPos = %col.getPosition();
		%obj.prevPosition = %obj.getPosition();
		%props = %obj.getItemProps();
		%props.uses--;
		%this.razeSchedule = %this.schedule(2250, onRaze, %obj, %col);
		if (isObject(%obj.client))
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Shaving...");
	}
	else
	{
		serverPlay3d(radioWaveExplosionSound, %obj.getHackPosition());
	}
}

function RazorImage::onRightClick(%this, %obj, %slot)
{
	if(%obj.getImageState(0) !$= "Ready")
		return;
	if(getField(%obj.character.appearance, 3) $= "")
	{
		if (isObject(%obj.client))
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>You're already bald!");
		return;
	}
	serverPlay3d(RazorStartSound, %obj.getHackPosition());
	%obj.setImageAmmo(0, 0);
	%obj.prevTargetPos = %obj.getPosition();
	%obj.prevPosition = %obj.getPosition();
	%this.razeSchedule = %this.schedule(1800, onRaze, %obj, %obj);
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Shaving...");
}

function RazorImage::onRaze(%this, %obj, %col)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead" || %obj.unconscious || !isObject(%col))
		return;
	if(%obj.getMountedImage(0) != %this)
		return;
	if(%obj.getPosition() !$= %obj.prevPosition || %col.getPosition() !$= %obj.prevTargetPos)
	{
		%obj.setImageAmmo(0, %props.uses >= 0);
		if (isObject(%obj.client))
			commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Action interrupted. Stand still!");
		%obj.prevPosition = "";
		%obj.prevTargetPos = "";
		return;
	}
	%obj.prevPosition = "";
	%obj.prevTargetPos = "";
	serverPlay3d(RazorHitSound, %col.getHackPosition());
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>Done!");

	if(isObject(%col.character))
	{
		%app = %col.character.appearance;
		%color = getField(%app, 7);
		%col.spawnFiber(%color);
		%col.spawnFiber(%color);
		%col.spawnFiber(%color);
		%col.spawnFiber(%color);
		%col.character.appearance = setField(%col.character.appearance, 3, "");
		%col.applyAppearance();
	}

	%obj.setImageAmmo(0, %props.uses >= 0);
}