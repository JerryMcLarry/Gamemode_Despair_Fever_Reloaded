datablock ItemData(CoatItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/coat.dts";
	image = CoatImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Coat";
	canDrop = true;

	hideAppearance = true; //hides arms, chest and pants

	waitForKiller = true; //Wait for killer to be picked before this can be picked up
};
datablock ShapeBaseImageData(CoatImage)
{
	item = CoatItem;
	shapeFile = $Despair::Path @ "res/shapes/items/coat.dts";
	doColorShift = false;
	mountPoint = 2;
	eyeOffset = "0 0 -50";
};

function CoatItem::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	fixArmReady(%obj);
	if (isObject(%obj.client))
		%obj.client.centerPrint("\c6Click to equip/unequip \c3" @ %this.uiName, 2);
}

//Functions defined by hats.cs
function CoatItem::onWear(%this, %player)
{
	if(isObject(%img = %player.getMountedImage(1)) && %img != %this.image)
	{
		%player.unMountImage(1);
		if (isObject(%player.client))
			%player.client.centerPrint("\c6You unequip \c3" @ %this.uiName, 2);
	}
	else
	{
		%player.mountImage(%this.image, 1);
		if (isObject(%player.client))
			%player.client.centerPrint("\c6You equip \c3" @ %this.uiName, 2);
	}
	%player.applyAppearance();
}

function CoatItem::onDrop(%this, %player, %index)
{
	%player.unMountImage(1);
	%player.applyAppearance();
	%player.tool[%index] = "";
	if(isObject(%client = %player.client))
		messageClient(%client, 'MsgItemPickup', '', %index, "");
}