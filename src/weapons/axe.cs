datablock ItemData(AxeItem)
{
	category = "DespairWeapon";
	classname = "DespairWeapon";

	shapeFile = $Despair::Path @ "res/shapes/weapons/Axe.dts";
	iconName = $Despair::Path @ "res/shapes/weapons/icon_sword";
	image = AxeImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";
	uiName = "Axe";
	canDrop = true;

	itemPropsClass = "MeleeProps";
	itemPropsAlways = true;
};

function AxeItem::onAdd(%this, %obj)
{
	parent::onAdd(%this, %obj);
	if(%obj.itemProps.bloody)
		%obj.playThread(0, "blood");
	else
		%obj.playThread(0, "root");
}

datablock ShapeBaseImageData(AxeImage)
{
	className = "WeaponImage";
	item = AxeItem;

	shapeFile = $Despair::Path @ "res/shapes/weapons/Axe.dts";
	doColorShift = true;
	colorShiftColor = "0.5 0.5 0.5 1";

	useCustomStates = true;
	type = "sharp";

	armReady = false;

	fireManual = true;

	windUp = 0.3;
	fireDelay = 0.6;
	fireScript = "onFire";
	meleeRange = 4;

	damage = 25;

	stateName[0]					= "Activate";
	stateAllowImageChange[0]		= 1;
	stateSequence[0]				= "root";
	stateTimeoutValue[0]			= 0.01;
	stateTransitionOnTimeOut[0]		= "CheckBlood";

	stateName[1]					= "CheckBlood";
	stateTransitionOnAmmo[1]		= "Blood";
	stateSequence[1]				= "root";

	stateName[2]					= "Blood";
	stateSequence[2]				= "blood";
};

function AxeImage::onMount(%image, %player, %slot)
{
	%props = %player.getItemProps();
	%player.setImageAmmo(%slot, %props.bloody);
	if(!%player.updateBloody)
	{
		%player.playThread(1, "1hpre1");
		%player.schedule(32, stopThread, 1);
	}
	%player.updateBloody = 0;
	if(isObject(%player.client))
		%player.client.play3d("bladeEquipSound", %player.getSlotTransform(0));
}

function AxeImage::onUnMount(%image, %player, %slot)
{
	cancel(%player.windUpSchedule);
}

function AxeImage::onWindUp(%image, %player)
{
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hpre" @ %player.swingType);
	%windUp = %image.windUp*1000;
	%player.lastFireTime = $Sim::Time + %windUp;
	%player.windUpSchedule = %image.schedule(%windUp, onFire, %player);
}

function AxeImage::onFire(%image, %player)
{
	cancel(%player.windUpSchedule);
	%player.swingType = getRandom(1, 2);
	%player.playThread(1, "1hswing" @ %player.swingType);
	%player.lastFireTime = $Sim::Time;
	fireMelee(%image, %player);
	%player.playAudio(1, "bladeSwingSound" @ getRandom(1, 3));
}

function AxeImage::onMeleeHit(%image, %player, %object, %position, %normal)
{
	if (!isObject(%object))
		return;

	%damage = %image.damage;
	%props = %player.getItemProps();
	if (%object.getType() & $TypeMasks::PlayerObjectType)
	{
		sprayBloodGush(%position, VectorScale(%normal, -13), %object);
		if(!%props.bloody)
		{
			%props.bloody = %object.health - %damage <= 0 || getRandom() < 0.7; //High chance to get bloody because sharp
			if(%props.bloody)
			{
				%player.updateBloody = 1;
				%player.unMountImage(0); %player.mountImage(%image, 0); //update blood
			}
		}

		if(%props.bloody && getRandom() < 0.7) //Another random chance to get bloody hand
		{
			%player.bloodyWriting = 2;
			%player.bloody["rhand"] = true;
			if (isObject(%player.client))
				%player.client.applyBodyParts();
		}

		if(%props.bloody && getRandom() < 0.7) //And random chance to get some blood on chest
		{
			%player.bloodyWriting = 2;
			%player.bloody["chest_front"] = true;
			if (isObject(%player.client))
				%player.client.applyBodyParts();
		}
		ServerPlay3D("BladeHitSound" @ getRandom(1, 2), %position);
		return %object.damage(%player, %position, %damage, %image.type);
	}
	if(%object.getType() & $TypeMasks::FxBrickObjectType && %object.getDataBlock().isDoor)
	{
		%dam = %object.doorDamage(1);
		if(%dam)
		{
			ServerPlay3D(WoodHitSound, %position);
			return %dam;
		}
	}
}