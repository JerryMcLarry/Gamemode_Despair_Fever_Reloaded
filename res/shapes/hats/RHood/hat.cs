datablock ItemData(HatRHoodItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./RHood.dts";
	image = HatRHoodImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat RHood";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
	replaceHair = "hair_messy";
};
datablock ShapeBaseImageData(HatRHoodImage)
{
	item = HatRHoodItem;
	shapeFile = "./RHood.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
