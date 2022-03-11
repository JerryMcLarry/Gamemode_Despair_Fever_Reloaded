datablock ItemData(HatFancyItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Fancy.dts";
	image = HatFancyImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Fancy";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
	replaceHairMale = "hair_messy";
	replaceHairFemale = "hair_ponytail";
};
datablock ShapeBaseImageData(HatFancyImage)
{
	item = HatFancyItem;
	shapeFile = "./Fancy.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
