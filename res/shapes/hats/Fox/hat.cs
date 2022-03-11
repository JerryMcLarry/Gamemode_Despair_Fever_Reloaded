datablock ItemData(HatFoxItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Fox.dts";
	image = HatFoxImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Fox";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
};
datablock ShapeBaseImageData(HatFoxImage)
{
	item = HatFoxItem;
	shapeFile = "./Fox.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
