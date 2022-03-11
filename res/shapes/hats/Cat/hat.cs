datablock ItemData(HatCatItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Cat.dts";
	image = HatCatImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Cat";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
};
datablock ShapeBaseImageData(HatCatImage)
{
	item = HatCatItem;
	shapeFile = "./Cat.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
