datablock ItemData(HatGangsterItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Gangster.dts";
	image = HatGangsterImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Gangster";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
	replaceHairMale = "hair_messy";
	replaceHairFemale = "hair_ponytail";
};
datablock ShapeBaseImageData(HatGangsterImage)
{
	item = HatGangsterItem;
	shapeFile = "./Gangster.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
