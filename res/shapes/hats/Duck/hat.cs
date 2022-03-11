datablock ItemData(HatDuckItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Duck.dts";
	image = HatDuckImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Duck";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
};
datablock ShapeBaseImageData(HatDuckImage)
{
	item = HatDuckItem;
	shapeFile = "./Duck.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
