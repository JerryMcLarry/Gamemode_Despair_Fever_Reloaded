datablock ItemData(HatRichardItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Richard.dts";
	image = HatRichardImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Richard";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	hidehair = true;
	disguiseName = "Rooster";
};
datablock ShapeBaseImageData(HatRichardImage)
{
	item = HatRichardItem;
	shapeFile = "./Richard.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
