datablock ItemData(HatNinjaItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Ninja.dts";
	image = HatNinjaImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Ninja";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	disguiseName = "Ninja";
	hidehair = true;
};
datablock ShapeBaseImageData(HatNinjaImage)
{
	item = HatNinjaItem;
	shapeFile = "./Ninja.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
