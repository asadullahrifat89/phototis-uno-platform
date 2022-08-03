var UnoAppManifest = {

    splashScreenImage: "Assets/SplashScreen.png",
    splashScreenColor: "transparent",
    displayName: "Phototis"

}

function exportImage(src, width, height, filter) {
    var canvas = document.createElement("canvas");
    canvas.height = height;
    canvas.width = width;
    var ctx = canvas.getContext('2d');

    var image = document.createElement("img");
    image.height = height;
    image.width = width;

    image.onload = function () {                
        ctx.filter = filter;
        ctx.drawImage(image, 0, 0, image.width * .6, image.height * .6);

        var dataUrl = canvas.toDataURL("image/png");
        window.open(dataUrl);
        //if (window.open(dataUrl)) {
        //    document.location.href = dataUrl;
        //}

        //return dataUrl;
    }

    //image.addEventListener('load', (e) => {
       
    //});
   
    image.src = src;
}
