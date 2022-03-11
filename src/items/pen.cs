datablock itemData(PenItem)
{
	shapeFile = $Despair::Path @ "res/shapes/items/pen.dts";
	image = PenImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = true;
	colorShiftColor = "0 0 1 1";
	uiName = "Pen";
	canDrop = true;

	itemPropsClass = "PenProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = false;
};

function PenProps::onAdd(%this)
{
	%this.ink = 10;
	%this.maxink = 10;
}

datablock ShapeBaseImageData(PenImage)
{
	item = PenItem;
	shapeFile = $Despair::Path @ "res/shapes/items/Pen.dts";
	doColorShift = true;
	colorShiftColor = "0 0 1 1";
	offset = "0 0 0";
	rotation = eulerToMatrix("-90 0 0");

	armReady = false;
};

function PenImage::onMount(%this, %obj, %slot)
{
	fixArmReady(%obj);
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'CenterPrint', "<color:FFFF00>This is a passive item.\nYou can /w[rite] [msg] on walls and paper with this.");
}

function PenImage::onUnMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (isObject(%obj.client))
		commandToClient(%obj.client, 'ClearCenterPrint');
}