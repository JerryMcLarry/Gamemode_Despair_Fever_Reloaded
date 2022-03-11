datablock ItemData(HatWizardItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./Wizard.dts";
	image = HatWizardImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat Wizard";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = false;
	hidehair = false;
	replaceHairMale = "hair_messy";
	replaceHairFemale = "hair_ponytail";
};
datablock ShapeBaseImageData(HatWizardImage)
{
	item = HatWizardItem;
	shapeFile = "./Wizard.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
