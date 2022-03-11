datablock ItemData(HatGangsterRedItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./GangsterRed.dts";
	image = HatGangsterRedImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Gangster Red";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
	replaceHairMale = "hair_messy";
	replaceHairFemale = "hair_ponytail";
};
datablock ShapeBaseImageData(HatGangsterRedImage)
{
	item = HatGangsterRedItem;
	shapeFile = "./GangsterRed.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
