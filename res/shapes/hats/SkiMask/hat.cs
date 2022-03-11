datablock ItemData(HatSkiMaskItem)
{
	category = "Hat";
	classname = "Hat";
	shapeFile = "./SkiMask.dts";
	image = HatSkiMaskImage;
	mass = 1;
	drag = 0.3;
	density = 0.2;
	elasticity = 0;
	friction = 1;
	doColorShift = false;
	uiName = "Hat SkiMask";
	canDrop = true;
	iconName = $Despair::Path @ "res/shapes/hats/icon_hat";

	disguise = true;
	hidehair = true;
	disguiseName = "Bandit";
};
datablock ShapeBaseImageData(HatSkiMaskImage)
{
	item = HatSkiMaskItem;
	shapeFile = "./SkiMask.dts";
	doColorShift = false;
	mountPoint = $headSlot;
	eyeOffset = "0 0 -50";
};
