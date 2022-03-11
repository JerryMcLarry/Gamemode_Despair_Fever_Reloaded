datablock AudioProfile(BladeHitSound1)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BladeHit1.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(BladeHitSound2)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BladeHit2.wav";
	description = audioClose3D;
	preload = true;
};

datablock AudioProfile(BladeSwingSound1)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BladeSwing1.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BladeSwingSound2)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BladeSwing2.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BladeSwingSound3)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BladeSwing3.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(BladeEquipSound)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BladeEquip.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(BluntHitSound1)
{
	fileName = $Despair::Path @ "res/sounds/weapons/PipeHit1.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(BluntHitSound2)
{
	fileName = $Despair::Path @ "res/sounds/weapons/PipeHit2.wav";
	description = audioClose3D;
	preload = true;
};
datablock AudioProfile(BluntHitSound3)
{
	fileName = $Despair::Path @ "res/sounds/weapons/PipeHit3.wav";
	description = audioClose3D;
	preload = true;
};

datablock AudioProfile(BluntSwingSound1)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BluntSwing1.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BluntSwingSound2)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BluntSwing2.wav";
	description = audioClosest3D;
	preload = true;
};
datablock AudioProfile(BluntSwingSound3)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BluntSwing3.wav";
	description = audioClosest3D;
	preload = true;
};

datablock AudioProfile(BluntEquipSound)
{
	fileName = $Despair::Path @ "res/sounds/weapons/BluntEquip.wav";
	description = audioClosest3D;
	preload = true;
};

datablock ItemData(noWeaponIcon)
{
	shapeFile = "base/data/shapes/empty.dts";
	iconName = $Despair::Path @ "res/shapes/weapons/icon_noWeapon";
	uiName = "No Weapon";
	isIcon = true;
	image = printGunImage;
};

function noWeaponIcon::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.currtool = -1;
	fixArmReady(%obj);
}

function DespairWeapon::onDrop(%this, %player, %index)
{
	%player.unMountImage(0);
	%player.tool[%player.weaponSlot] = noWeaponIcon.getID();
	%player.currtool = -1;
	if(isObject(%client = %player.client))
	{
		messageClient(%client, 'MsgItemPickup', '', %player.weaponSlot, noWeaponIcon.getID(), true);
		//%client.applyBodyParts();
	}
}

function MeleeProps::onAdd(%this)
{
	%this.bloody = false;
}

function Player::tryStartFireWeapon(%player, %manual)
{
	if ($DefaultMiniGame.noWeapons || %player.noWeapons)
		return;

	%image = %player.getMountedImage(0);

	if (isEventPending(%player.critLoop) && !%image.gun)
		return;

	if (%manual || (!%image.fireManual && %player.getImageTrigger(0)))
		%player.fireWeapon(false, %manual);
}

function Player::fireWeapon(%player, %ignoreTime, %manual)
{
	cancel(%player.fireSchedule);

	if ($DefaultMiniGame.noWeapons || %player.noWeapons)
		return;

	if (%player.health <= 0 || (!%player.getImageTrigger(0) && !%manual))
		return;

	%image = %player.getMountedImage(0);

	if (!%image)
		return;

	if(!%image.gun && isObject(%player.character) && %player.character.trait["Bodybuilder"])
		%extra = -0.05; // a BIT faster

	if(!%image.gun && %player.mood !$= "")
	{
		%mood = getMoodName(%player.mood);
		if (%mood $= "sad")
			%extra += 0.1; // slower
		else if (%mood $= "depressed")
			%extra += 0.2; // significantly slower
	}

	%time = %image.fireDelay * (%player.swingSpeedMod + %extra);
	if ($Sim::Time - %player.lastFireTime < %time && !%ignoreTime)
	{
		if (!%image.fireManual)
		{
			%time -= $Sim::Time - %player.lastFireTime;
			%player.fireSchedule = %player.schedule(%time * 1000, fireWeapon, true);
		}
		return;
	}

	%func = %image.fireScript;
	if(%image.fireScript $= "")
		%func = "onFire";

	if (!eval("return %image."@%func@"(%player);"))
		return;

	%player.lastFireTime = $Sim::Time;
	if(isObject(%client = %player.client))
		RS_Log("[DMGLOG]" SPC %client.getPlayerName() SPC "[" @ %client.getBLID() @ "] swung their " @ %image.item.uiName, "\c4");
	if(getRandom() < 0.2)
		%player.spawnFiber();

	if (!%image.fireManual)
		%player.fireSchedule = %player.schedule(%time * 1000, fireWeapon, true);
}

function fireMelee(%image, %player)
{
	%angle = %image.meleeAngle;
	%range = %image.meleeRange;
	%count = %image.meleeCount;
	// if (%angle $= "") %angle = 1.57079 * 0.5;
	if (%angle $= "") %angle = 1.57079 * 0.75;
	if (%range $= "") %range = 3.5;
	if (%count $= "") %count = 3; // 3;

	%start = %angle * -0.5;
	%i = %count;
	%count--;
	while (%i-- >= 0)
	{
		%time = mAbs(100 * ((1 - %i) / %count));
		%yaw = %start + %angle * (%i / %count);
		schedule(%time, %player, fireMeleeCheck, %image, %player, %range, %yaw);
	}
}

function fireMeleeCheck(%image, %player, %range, %yaw)
{
	if (%player.health <= 0 || %player.getMountedImage(0) != %image)
		return;

	%origin = %player.getEyePoint();
	// %b = VectorAdd(%a, MatrixMulVector("0 0 0 " @ %player.getUpVectorHack() @ " " @ %yaw,
	//         VectorScale(%player.getAimVector(), %range)));
	%a = VectorAdd(%origin, MatrixMulVector("0 0 0 0 0 1 " @ (%yaw * 2.5),
			VectorScale(%player.getAimVector(), 0.5)));
	%b = VectorAdd(%origin, MatrixMulVector("0 0 0 0 0 1 " @ %yaw,
			VectorScale(%player.getAimVector(), %range)));

	%ray = containerRayCast(%a, %b, $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType, %player);
	%col = firstWord(%ray);
	//schedule(1500, 0, freeLine, drawLine("", %a, %b, "1 0 0 1"));

	if (!%col)
		return;
	
	if (!(%col.getType() & $TypeMasks::PlayerObjectType))
	{
		if ($Sim::Time - %player.lastMeleeTerrainHit < 0.1)
			return;
		%player.lastMeleeTerrainHit = $Sim::Time;
	}

	if ($Sim::Time - %player.lastMeleeHitTo[%col] < 0.1)
		return;
	%player.lastMeleeHitTo[%col] = $Sim::Time;

	%image.onMeleeHit(%player, %col, getWords(%ray, 1, 3), getWords(%ray, 4, 6));
}

package DespairWeapons
{
	function Armor::onTrigger(%data, %player, %slot, %state)
	{
		Parent::onTrigger(%data, %player, %slot, %state);

		%image = %player.getMountedImage(0);

		if (!isObject(%image))
			return;
		if(isFunction(%image.getName(), "onRightClick") && %slot == 4 && %state)
			%image.onRightClick(%player, 0);

		if (!%image.useCustomStates)
			return;
		if (%slot != 0)
			return;
		if (%state)
			%player.tryStartFireWeapon(true);
		else
			cancel(%player.fireSchedule);
	}
};

activatePackage("DespairWeapons");