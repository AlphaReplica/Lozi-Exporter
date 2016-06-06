var loziobject;
	
Lozi.load('examples/mummy/mummy_asset.js',function(obj)
{
	loziobject = obj.object;
	scene.add(loziobject);
		
	loziobject.setScale(3,3,3);
		
	loziobject.position.y = -250;
		
	for(var num = 0; num < loziobject.animations().length; num++)
	{
		loziobject.animations()[num]._actions[0].play();
	}
	
	onLoaded();
},onProgress);
		
function render()
{
	renderer.render(scene, camera);
		
	if(loziobject)
	{
		loziobject.updateAnimations(0.75 * clock.getDelta());
	}
}