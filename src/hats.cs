$pattern = $Despair::Path @ "res/shapes/hats/*.cs";
echo("\c4Executing hats...");
for ( $file = findFirstFile( $pattern ) ; $file !$= "" ; $file = findNextFile( $pattern ) )
{
	exec( $file );
}
echo("\c4Done!");

datablock ItemData(noHatIcon)
{
	shapeFile = "base/data/shapes/empty.dts";
	iconName = $Despair::Path @ "res/shapes/hats/icon_nohat";
	uiName = "No Hat";
	isIcon = true;
	image = printGunImage;
};

function noHatIcon::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.currtool = -1;
	fixArmReady(%obj);
}

function Hat::onPickup(%this, %obj, %player)
{
	if(%player.tool[%player.hatSlot] != nameToID(noHatIcon))
		return false;

	%id = %this.getId();
	%player.tool[%player.hatSlot] = %id;
	if (isObject(%player.client))
		messageClient(%player.client, 'MsgItemPickup', '', %player.hatSlot, %id, false);
	if(isObject(%obj))
		%obj.delete();
	return true;
}

function Hat::onUse(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.currtool = %slot;
	fixArmReady(%obj);
	if (isObject(%obj.client))
		%obj.client.centerPrint("\c6Click to equip/unequip \c3" @ %this.uiName, 2);
}

function Hat::onWear(%this, %player)
{
	if(isObject(%img = %player.getMountedImage(2)) && %img != %this.image)
	{
		%player.unMountImage(2);
		if (isObject(%player.client))
			%player.client.centerPrint("\c6You unequip \c3" @ %this.uiName, 2);
	}
	else
	{
		%player.mountImage(%this.image, 2);
		if (isObject(%player.client))
			%player.client.centerPrint("\c6You equip \c3" @ %this.uiName, 2);
	}
	%player.applyAppearance();
}

function Hat::onDrop(%this, %player, %index)
{
	%player.tool[%player.hatSlot] = NoHatIcon.getID();
	%player.unMountImage(2);
	%player.currtool = -1;
	%player.applyAppearance();
	if(isObject(%client = %player.client))
	{
		messageClient(%client, 'MsgItemPickup', '', %player.hatSlot, NoHatIcon.getID());
	}
}