datablock ItemData(BananaItem)
{
	shapeFile = $Despair::Path @ "res/shapes/food/Banana.dts";
	mass = 1;
	density = 0.4;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Banana";

	image = BananaImage;
	canDrop = true;
	
	waitForKiller = false; //Wait for killer to be picked before this can be picked up
};

datablock ShapeBaseImageData(BananaImage)
{
	shapeFile = $Despair::Path @ "res/shapes/food/Banana.dts";

	emap = true;
	mountPoint = 0;

	className = "WeaponImage";
	item = BananaItem;

	armReady = true;

	stateName[0]					= "Activate";
	stateTransitionOnTimeOut[0]		= "Ready";
	stateAllowImageChange[0]		= true;
	stateTimeoutValue[0]			= 0.5;

	stateName[1]					= "Ready";
	stateTransitionOnTriggerDown[1]	= "Fire";
	stateAllowImageChange[1]		= true;

	stateName[2]					= "Fire";
	stateTransitionOnTimeout[2]		= "Ready";
	stateAllowImageChange[2]		= true;
	stateScript[2]					= "onEat";
	stateTimeoutValue[2]			= 1;
};

datablock ItemData(BananaPeelItem)
{
	shapeFile = $Despair::Path @ "res/shapes/food/BananaPeel.dts";
	mass = 0.5;
	density = 0.4;
	elasticity = 0.1;
	friction = 0.7;
	emap = true;

	uiName = "Banana Peel";

	image = BananaPeelImage;
	canDrop = true;

	slip = 1;

	waitForKiller = false; //Wait for killer to be picked before this can be picked up
};

datablock ShapeBaseImageData(BananaPeelImage)
{
	shapeFile = $Despair::Path @ "res/shapes/food/BananaPeel.dts";

	emap = true;
	mountPoint = 0;

	className = "WeaponImage";
	item = BananaItem;

	armReady = true;
};

function BananaImage::onEat(%this, %obj, %slot)
{
	%obj.unMountImage(0);
	%obj.setTool(%obj.currTool, BananaPeelItem);
	%obj.lastEat = $Sim::Time;
	serverPlay3d("EatSound", %obj.getEyePoint());
	%obj.addMood(3, "Mmm, that was tasty!");
}