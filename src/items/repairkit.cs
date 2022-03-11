datablock ItemData(RepairkitItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/screwdriver.dts";

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "Repair Kit";
	// iconName = "Add-Ons/Item_Repairkit/Icon_Repairkit";
	doColorShift = true;
	colorShiftColor = "0.4 0.4 0.4 1";

	image = RepairkitImage;

	canDrop = true;
};

datablock ShapeBaseImageData(RepairkitImage)
{
	className = "WeaponImage";
	shapeFile = $Despair::Path @ "res/shapes/items/screwdriver.dts";

	rotation = eulerToMatrix("0 0 90");

	item = RepairkitItem;
	armReady = true;

	doColorShift = RepairkitItem.doColorShift;
	colorShiftColor = RepairkitItem.colorShiftColor;
};

function RepairkitImage::onMount(%this, %obj, %slot)
{
	Parent::onMount(%this, %obj, %slot);
}

function Player::RepairkitDoAfter(%this, %time, %target, %ticks, %prevPosition, %done)
{
	cancel(%this.RepairkitDoAfter);
	if (%ticks $= "")
		%ticks = 1;

	if (%prevPosition !$= "" && %this.getPosition() !$= %prevPosition || %this.getMountedImage(0) != RepairkitImage.getID() || !isObject(%target))
	{
		if (isObject(%this.client))
			%this.client.centerPrint("\c6Repair Kit action interrupted!", 2);
		return -1; //Fail
	}
	if (%done >= %ticks)
	{
		%target.broken = false;
		serverPlay3d(DoorLockSound, %target.getWorldBoxCenter(), 1);
		%this.playThread(2, "rotCW");
		if (isObject(%this.client))
			%this.client.centerPrint("\c6You fix the door.", 2);
		return 1; //Success
	}
	%this.playThread(2, "shiftRight");
	serverPlay3d(DoorPickSound @ getRandom(1,3), %target.getWorldBoxCenter(), 1);
	for (%i = 0; %i < %done % 4; %i++)
	{
		%dots = %dots @ ".";
	}
	%this.client.centerPrint("\c6Fixing the door" @ %dots, 2);
	%prevPosition = %this.getPosition();
	%timefraction = mFloor(%time/%ticks);
	%this.RepairkitDoAfter = %this.schedule(%timefraction, RepairkitDoAfter, %time, %target, %ticks, %prevPosition, %done++);
}

package RepairkitPackage
{
	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		Parent::onTrigger(%this, %obj, %slot, %state);

		if (!%state || %obj.getMountedImage(0) != RepairkitImage.getID() || %slot != 0)
			return;

		%a = %obj.getEyePoint();
		%b = vectorAdd(%a, vectorScale(%obj.getEyeVector(), 4));

		%mask =
			$TypeMasks::FxBrickObjectType |
			$TypeMasks::PlayerObjectType;

		%ray = containerRayCast(%a, %b, %mask, %obj);

		if (!%ray)
			return;

		%props = %obj.getItemProps();

		%data = %ray.getDataBlock();

		if (!%data.isDoor || %data.isOpen || isEventPending(%obj.RepairkitDoAfter))
			return;

		if (%slot == 0) //do the thang
		{
			if (%ray.broken)
			{
				serverPlay3d(DoorJiggleSound, %ray.getWorldBoxCenter(), 1);
				%obj.playThread(2, "activate");
				%obj.RepairkitDoAfter(7000, %ray, 7);
			}
			else
			{
				serverPlay3d(DoorJiggleSound, %ray.getWorldBoxCenter(), 1);
				%obj.playThread(2, "shiftRight");
				if (isObject(%obj.client))
					%obj.client.centerPrint("\c6The door is already fixed.", 2);
			}
		}
	}
};

activatePackage("RepairkitPackage");
