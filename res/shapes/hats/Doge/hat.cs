datablock ItemData(HatDogeItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Doge.dts";
	image = HatDogeImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Doge";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	disguiseName = "Woofman";
	hidehair = true;
};
datablock ShapeBaseImageData(HatDogeImage)
{
	item = HatDogeItem;
	shapeFile = "./Doge.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
