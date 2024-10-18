﻿


function format() {
    debugger
    // Hàm định dạng tiền tệ Việt Nam
    function formatCurrency(value) {
        return parseInt(value).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }

    // Lấy tất cả các phần tử có class 'price'
    const prices = document.querySelectorAll('.price');

    // Lặp qua từng phần tử và định dạng giá trị dựa trên thuộc tính 'data-price'
    prices.forEach(function (priceElement) {
        const priceValue = priceElement.getAttribute('data-price');
        if (priceValue) {
            priceElement.innerText = formatCurrency(priceValue);  // Định dạng và hiển thị lại giá trị
        }
    });

}



function encryptData(data, key) {
    return CryptoJS.AES.encrypt(JSON.stringify(data), key).toString();
}
//Giai mã hóa giỏ hàng 

function decryptData(encryptedData, key) {
    const bytes = CryptoJS.AES.decrypt(encryptedData, key);
    return JSON.parse(bytes.toString(CryptoJS.enc.Utf8));
}
function saveCartToLocalStorage(cartItems) {
    const cartJson = JSON.stringify(cartItems);
    localStorage.setItem("cart", cartJson);
}

function getCartFromLocalStorage() {
    const cartJson = localStorage.getItem("cart");
    if (cartJson) {
        return JSON.parse(cartJson);
    }
    return [];
}


function AddToCart(id) {

    
}


function selectSize(size) { 
    $("#selectedSize")
}