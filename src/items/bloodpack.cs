datablock ItemData(bloodpackItem)
{
    category = "Weapon";  // Mission editor category
    className = "Weapon"; // For inventory system

    // Basic Item Properties
    shapeFile = $Despair::Path @ "res/shapes/items/bloodpack.dts";
    rotate = false;
    mass = 1;
    density = 0.4;
    elasticity = 0.2;
    friction = 0.6;
    emap = true;

    //gui stuff
    uiName = "Bloodpack";
    iconName = $Despair::Path @ "res/shapes/items/Icon_bloodpack";
    doColorShift = false;

    // Dynamic properties defined by the scripts
    image = bloodpackImage;
    canDrop = true;

    waitForKiller = true;
    customPickupMultiple = true;
};

datablock ShapeBaseImageData(bloodpackImage)
{
    // Basic Item properties
    shapeFile = $Despair::Path @ "res/shapes/items/bloodpack.dts";
    emap = true;

    // Specify mount point & offset for 3rd person, and eye offset
    // for first person rendering.
    mountPoint = 0;
    offset = "0.05 0.1 0";
    eyeOffset = 0; //"0.7 1.2 -0.25";
    rotation = eulerToMatrix("0 -90 0");

    className = "WeaponImage";
    item = bloodpackItem;

    //raise your arm up or not
    armReady = true;

    doColorShift = false;

    // Initial start up state
    stateName[0] = "Activate";
    stateTransitionOnTimeOut[0] = "Ready";
    stateAllowImageChange[0] = true;
    stateTimeoutValue[0] = 0.5;

    stateName[1] = "Ready";
    stateTransitionOnTriggerDown[1] = "Fire";
    stateAllowImageChange[1] = true;

    stateName[2] = "Fire";
    stateTransitionOnTimeout[2] = "Ready";
    stateAllowImageChange[2] = true;
    stateScript[2] = "onUse";
    stateTimeoutValue[2] = 1;
};

function bloodpackImage::onUse(%this, %obj, %slot)
{
    sprayBloodWide(%obj.getHackPosition(), VectorScale(%obj.getForwardVector(), 10), %obj);
    sprayBloodWide(%obj.getHackPosition(), VectorScale(%obj.getForwardVector(), 15), %obj);
    sprayBloodWide(%obj.getHackPosition(), VectorScale(%obj.getForwardVector(), 20), %obj);
    serverPlay3d(bloodSpillSound, %obj.getHackPosition());

    %obj.bloody["lhand"] = true;
    %obj.bloody["rhand"] = true;
    %obj.bloody["chest_front"] = true;
    if (isObject(%obj.client))
        %obj.client.applyBodyParts();

    %obj.unMountImage(0);
    %obj.removeTool(%obj.currTool);
}