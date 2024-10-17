let moon = document.querySelectorAll(".moon");

moon[0].onclick = function () {
  if (moon[0].src.includes("moon.svg")) {
    document.body.classList.add("dark");
    moon[0].src = "images/sun-light.svg";
  } else {
    document.body.classList.remove("dark");
    moon[0].src = "images/moon.svg";
  }
};

moon[1].onclick = function () {
  if (moon[1].src.includes("moon.svg")) {
    document.body.classList.add("dark");
    moon[1].src = "images/sun-light.svg";
  } else {
    document.body.classList.remove("dark");
    moon[1].src = "images/moon.svg";
  }
};

let loader = document.querySelector(".loader");
let hidden = document.querySelector(".hidden");

setTimeout(function () {
  loader.style.display = "none";
  document.body.classList.add("appear");
  hidden.classList.add("appear");
}, 2000);
