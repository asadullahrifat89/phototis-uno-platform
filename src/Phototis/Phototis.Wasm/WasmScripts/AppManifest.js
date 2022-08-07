var UnoAppManifest = {
    splashScreenImage: "Assets/SplashScreen.png",
    splashScreenColor: "white",
    displayName: "Phototis"
}

function exportImage(id, filter, angle, src) {

    var image = new Image();
    image.style = "object-fit:contain";

    var canvas = document.createElement("canvas");

    canvas.style = "object-fit:contain";

    image.onload = function () {

        // flip height and width according to rotation angle
        if (angle == 90 || angle == 270) {
            canvas.height = this.width;
            canvas.width = this.height;
        }
        else {
            canvas.height = this.height;
            canvas.width = this.width;
        }

        var ctx = canvas.getContext('2d');

        ctx.filter = filter;

        if (angle > 0) {
            if (angle == 90 || angle == 270) {
                // translate to center-canvas 
                // the origin [0,0] is now center-canvas
                ctx.translate(canvas.width / 2, canvas.height / 2);

                // roate the canvas by angle
                ctx.rotate(DegToRad(angle));

                // draw the image
                // since images draw from top-left offset the draw by 1/2 width & height
                ctx.drawImage(image, -image.width / 2, -image.height / 2);

                // un-rotate the canvas by -angle
                ctx.rotate(-DegToRad(angle));

                // un-translate the canvas back to origin==top-left canvas
                ctx.translate(-canvas.width / 2, -canvas.height / 2);
            }
            else {
                // Translate to the center point of our image 
                ctx.translate(image.width * 0.5, image.height * 0.5);

                // Perform the rotation  
                ctx.rotate(DegToRad(angle));

                // Translate back to the top left of our image  
                ctx.translate(-image.width * 0.5, -image.height * 0.5);

                drawImageProp(ctx, image, 0, 0, this.width, this.height);
            }
        }
        else {
            drawImageProp(ctx, image, 0, 0, this.width, this.height);
        }

        var dataUrl = canvas.toDataURL("image/jpg");

        // Create a link and set the URL using
        const link = document.createElement("a");
        link.style.display = "none";
        link.href = dataUrl;
        link.download = id + ".jpg";

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

function DegToRad(d) {
    // Converts degrees to radians  
    return d * 0.01745;
}

// ctx is canvas 2D context
// angle is rotation in radians
// image is the image to draw
function drawToFitRotated(ctx, angle, image) {
    var dist = Math.sqrt(Math.pow(ctx.canvas.width / 2, 2) + Math.pow(ctx.canvas.height / 2, 2));
    var imgDist = Math.min(image.width, image.height) / 2;
    var minScale = dist / imgDist;
    var dx = Math.cos(angle) * minScale;
    var dy = Math.sin(angle) * minScale;
    ctx.setTransform(dx, dy, -dy, dx, ctx.canvas.width / 2, ctx.canvas.height / 2);
    ctx.drawImage(image, -image.width / 2, - image.height / 2);
    ctx.setTransform(1, 0, 0, 1, 0, 0);
}

function drawImageProp(ctx, img, x, y, w, h, offsetX, offsetY) {

    if (arguments.length === 2) {
        x = y = 0;
        w = ctx.canvas.width;
        h = ctx.canvas.height;
    }

    // default offset is center
    offsetX = typeof offsetX === "number" ? offsetX : 0.5;
    offsetY = typeof offsetY === "number" ? offsetY : 0.5;

    // keep bounds [0.0, 1.0]
    if (offsetX < 0) offsetX = 0;
    if (offsetY < 0) offsetY = 0;
    if (offsetX > 1) offsetX = 1;
    if (offsetY > 1) offsetY = 1;

    var iw = img.width,
        ih = img.height,
        r = Math.min(w / iw, h / ih),
        nw = iw * r,   // new prop. width
        nh = ih * r,   // new prop. height
        cx, cy, cw, ch, ar = 1;

    // decide which gap to fill    
    if (nw < w) ar = w / nw;
    if (Math.abs(ar - 1) < 1e-14 && nh < h) ar = h / nh;  // updated
    nw *= ar;
    nh *= ar;

    // calc source rectangle
    cw = iw / (nw / w);
    ch = ih / (nh / h);

    cx = (iw - cw) * offsetX;
    cy = (ih - ch) * offsetY;

    // make sure source rectangle is valid
    if (cx < 0) cx = 0;
    if (cy < 0) cy = 0;
    if (cw > iw) cw = iw;
    if (ch > ih) ch = ih;

    // fill image in dest. rectangle
    ctx.drawImage(img, cx, cy, cw, ch, x, y, w, h);
}
