datablock ItemData(HatSunglassesItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Sunglasses.dts";
	image = HatSunglassesImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Sunglasses";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
};
datablock ShapeBaseImageData(HatSunglassesImage)
{
	item = HatSunglassesItem;
	shapeFile = "./Sunglasses.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
