
function change_theme(themeId) {
    fetch('https://itu-sdbg-s2020.now.sh/api/themes')
     .then((response) => {
        return response.json();
     })
     .then((json) => {
        return json.themes[themeId];
     })
     .then((theme) => {
        changeThemeTo(theme.styles);
        const currentTheme = document.getElementById("currentTheme");
        currentTheme.innerHTML = "Tema: " + theme.name;
     })
     .catch((reason) => {
        console.log(reason);
     });
}

function changeThemeToDefault(){
    let style = {
        fontFamily: "serif",
        fontName: 'Noto Serif',
        primaryColor: "000000",
        secondaryColor: "fffff4"
    };
    const currentTheme = document.getElementById("currentTheme");
    currentTheme.innerHTML = "Tema: Standard";
    changeThemeTo(style);
}

function changeThemeTo(style){
    let elements = document.getElementsByClassName('themed');
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.fontFamily = style.fontName + ", " + style.fontFamily;
        elements[i].style.color = "#" + style.primaryColor;
        elements[i].style.backgroundColor = "#" + style.secondaryColor;
    }
}

function updatePos(){
    const uploadArea = document.getElementById("uploadArea");
    const pictureText = document.getElementById("userText");

    if(uploadArea.textSize.value > 48){
        uploadArea.textSize.value = 48;
    }
    else if(uploadArea.textSize.value < 10){
        uploadArea.textSize.value = 10;
    }

    pictureText.style.top = uploadArea.vertical.valueAsNumber + 16 + "px";  // + 16 is for padding
    pictureText.style.left = uploadArea.horizontal.valueAsNumber + 16 + "px";

    pictureText.style.fontSize = uploadArea.textSize.value + "px";
    pictureText.style.transform = 'rotate(' + uploadArea.rotation.value + 'deg)';
}

function updateText(){
    const uploadArea = document.getElementById("uploadArea");
    const pictureText = document.getElementById("userText");

    pictureText.innerHTML = uploadArea.textInput.value;
    pictureText.style.color = uploadArea.colorSelect.value;

    updatePos();
}

var picture;
//load a file and show a preview
// taken from: https://stackoverflow.com/questions/22087076/how-to-make-a-simple-image-upload-using-javascript-html
window.addEventListener('load', function() {
    document.querySelector('input[type="file"]').addEventListener('change', function() {
        if (this.files && this.files[0]) {
            picture = document.getElementById("myImg");
            picture.src = URL.createObjectURL(this.files[0]);

        }
    });
});

function scrollToTop(){
    window.scrollTo({ top: 0, behavior: 'smooth' })
}

function scrollToUpload(){
    if (editToggle){
        toggleEditMode();
    }
    let focusArea = document.getElementById("uploadArea");
    let normalColor = focusArea.style.color;
    focusArea.style.backgroundColor = "#B8B8B8";
    window.scrollTo(0,document.body.scrollHeight);
    setTimeout(function () {
        focusArea.style.backgroundColor = normalColor;
    }, 300);

}

function scrollToBottomOfAlbum(){
    var box = document.getElementById("splitLine");
    box.scrollIntoView({behavior: "smooth", block: "end"});
}


var pictureElements = [];

class Picture{
    constructor(imgSrc, caption, userText){
        this.imgSrc = imgSrc;
        this.caption = caption;
        this.userText = userText;
    }
}

function createPicture(picture){
    const newSinglePicture = document.createElement("div");
    const newImg = document.createElement("img");
    const caption = document.createElement("p");

    const editContainer = document.createElement("div");
    const bottomContainer = document.createElement("div");
    const moveLeft = document.createElement("img");
    const moveRight = document.createElement("img");
    const deleteThisImage = document.createElement("img");

    newSinglePicture.classList.add("singlePicture");
    newImg.src = picture.imgSrc;
    caption.innerHTML = picture.caption;

    moveLeft.src = "assets/ic_arrow_back_48px.svg";
    moveRight.src = "assets/ic_arrow_forward_48px.svg";
    deleteThisImage.src = "assets/ic_delete_48px.svg";

    editContainer.classList.add("editMode");
    moveLeft.classList.add("genericButton", "box");
    moveRight.classList.add("genericButton", "box");
    deleteThisImage.classList.add("genericButton", "box");

    moveLeft.addEventListener("click", function(){
        moveImage(newSinglePicture, -1);
    });
    moveRight.addEventListener("click", function(){
        moveImage(newSinglePicture, 1);
    });
    deleteThisImage.addEventListener("click", function(){
        deleteImage(newSinglePicture);
    });

    picture.userText.removeAttribute("id");

    newSinglePicture.appendChild(newImg);
    newSinglePicture.appendChild(picture.userText);
    newSinglePicture.appendChild(bottomContainer);

    bottomContainer.appendChild(caption);
    bottomContainer.appendChild(editContainer);

    editContainer.appendChild(moveLeft);
    editContainer.appendChild(deleteThisImage);
    editContainer.appendChild(moveRight);

    pictureElements.push(newSinglePicture);
    document.getElementById("imagesHere").appendChild(newSinglePicture);
}


//Includes a few default images, to quickly show how the album looks. For ease of use for examinators
//All default pictures used is taken from https://unsplash.com/ and used under their license https://unsplash.com/license which states:
//All photos published on Unsplash can be used for free. You can use them for commercial and noncommercial purposes.
//You do not need to ask permission from or provide credit to the photographer or Unsplash, although it is appreciated when possible.
function addDefaultImgs(){
    const sources = ["pictures/picture1.jpg", "pictures/picture2.jpg", "pictures/picture3.jpg", "pictures/picture4.jpg", "pictures/picture5.jpg", "pictures/picture6.jpg"];
    for (let i = 0; i < sources.length; i++) {
        let picture = new Picture(sources[i], "Standard Billede " + (i+1), document.createElement("userText"));
        createPicture(picture);
    }
}

function addCurrentPictureToAlbum(event){
    event.preventDefault();

    if(editToggle) {
        alert("Du kan ikke tilføje et billede i redigerings tilstand");
        return;
    }

    if(document.getElementById("myImg").style.visibility != 'visible'){
        alert("Du mangler et billede!");
        return;
    }

    const uploadArea = document.getElementById("uploadArea");

    const imgSrc = uploadArea.myImg.src;
    const caption = uploadArea.captionInput.value;
    const userText = document.getElementById("userText").cloneNode(true);
    const picture = new Picture(imgSrc, caption, userText);
    createPicture(picture);

    document.getElementById("uploadArea").reset();
    updateText();
    scrollToBottomOfAlbum();
    document.getElementById("myImg").style.visibility = 'hidden';

}

function moveImage(imageElement, distance){
    let idx = pictureElements.indexOf(imageElement);
    let otherIdx = idx + distance;

    if(otherIdx < 0){ otherIdx = 0; }
    else if(otherIdx >= pictureElements.length){ otherIdx = pictureElements.length - 1; }

    const otherElement = pictureElements[otherIdx];
    pictureElements[otherIdx] = pictureElements[idx];
    pictureElements[idx] = otherElement;

    reOrderPictures();
}

function deleteImage(imageElement){
    const indexOfDeletedElement = pictureElements.indexOf(imageElement)
    pictureElements.splice(indexOfDeletedElement,1,);
    imageElement.parentNode.removeChild(imageElement);
    reOrderPictures();
}

function reOrderPictures() {
    const imagesHere = document.getElementById("imagesHere");
    for (var i = 0; i < pictureElements.length; i++) {
        imagesHere.appendChild(pictureElements[i]);
    }
}

var editToggle = false;
function toggleEditMode(){
    let display;
    let editButton = document.getElementById("editButton");
    if(editToggle){
        display = "none";
        editButton.classList.remove("activeEdit");
        editButton.firstElementChild.src = "assets/ic_mode_edit_48px.svg";
    } else {
        display = "flex";
        editButton.classList.add("activeEdit");
        editButton.firstElementChild.src = "assets/ic_check_48px.svg";
    }
    editToggle = !editToggle;

    const editModeElements = document.getElementsByClassName("editMode");
    for (let i = 0; i < editModeElements.length; i++) {
        editModeElements[i].style.display = display;
    }
}

/* When the user clicks on the button,
toggle between hiding and showing the dropdown content */
function openDropdown() {
    document.getElementById("themeDropDown").classList.toggle("show");
}

// Close the dropdown if the user clicks outside of it
window.onclick = function(event) {
    if (!event.target.matches('.dropbtn')) {
        let dropdowns = document.getElementsByClassName("themeMenuContent");
        for (let i = 0; i < dropdowns.length; i++) {
            dropdowns[i].classList.remove('show');
        }
    }
}

function makeVisible(element) {
    document.getElementById(element).style.visibility = 'visible';
}