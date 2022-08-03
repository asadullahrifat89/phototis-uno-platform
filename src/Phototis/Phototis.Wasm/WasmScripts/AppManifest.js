var UnoAppManifest = {

    splashScreenImage: "Assets/SplashScreen.png",
    splashScreenColor: "transparent",
    displayName: "Phototis"

}

function exportImage(id, width, height, filter, src) {
    var canvas = document.createElement("canvas");
    canvas.height = height;
    canvas.width = width;
    canvas.style = "object-fit:contain";
    var ctx = canvas.getContext('2d');

    var image = document.createElement("img");
    image.height = height;
    image.width = width;
    image.style = "object-fit:contain";

    image.onload = function () {
        ctx.filter = filter;
        ctx.drawImage(image, 0, 0, image.width, image.height);

        var dataUrl = canvas.toDataURL("image/png");

        // Create a link and set the URL using
        const link = document.createElement("a");
        link.style.display = "none";
        link.href = dataUrl;
        link.download = id + ".png";

        // It needs to be added to the DOM so it can be clicked
        document.body.appendChild(link);
        link.click();

        // To make this work on Firefox we need to wait
        // a little while before removing it.
        setTimeout(() => {
            URL.revokeObjectURL(link.href);
            link.parentNode.removeChild(link);
        }, 0);
    }

    image.src = src;
}
