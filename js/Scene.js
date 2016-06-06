var camera, controls, scene, renderer;
var clock = new THREE.Clock();

function init()
{
	scene    = new THREE.Scene();
	renderer = new THREE.WebGLRenderer();
	
	renderer.shadowMap.enabled = true;
	renderer.shadowMap.type    = THREE.PCFSoftShadowMap;
	
	camera				   = new THREE.PerspectiveCamera( 60, window.innerWidth / window.innerHeight, 1, 2000 );
	camera.position.z	   = 500;
	controls 			   = new THREE.OrbitControls( camera, renderer.domElement );
	controls.enableDamping = true;
	controls.dampingFactor = 0.25;
	controls.enableZoom    = false;
	
	
	renderer.setPixelRatio( window.devicePixelRatio );
	renderer.setSize( window.innerWidth, window.innerHeight );
	
	document.body.appendChild( renderer.domElement );
	
	window.addEventListener( 'resize', onWindowResize, false );
}

function onWindowResize()
{
	camera.aspect = window.innerWidth / window.innerHeight;
	camera.updateProjectionMatrix();
	renderer.setSize( window.innerWidth, window.innerHeight );
}

function animate()
{
	requestAnimationFrame( animate );
	controls.update(); // required if controls.enableDamping = true, or if controls.autoRotate = true
	render();
}

function render()
{
	renderer.render(scene, camera);
}


init();

animate();