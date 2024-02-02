// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
const firebaseConfig = {
  // apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  // authDomain: 'mern-estate.firebaseapp.com',
  // projectId: 'mern-estate',
  // storageBucket: 'mern-estate.appspot.com',
  // messagingSenderId: '1078482850952',
  // appId: '1:1078482850952:web:28f19139ab77246602fb3d',

  apiKey: "AIzaSyCl7A-fUStwqZTPmzes6lazVVYrsL8VLVM",
  authDomain: "real-estate-test-eb656.firebaseapp.com",
  databaseURL: "https://real-estate-test-eb656-default-rtdb.firebaseio.com",
  projectId: "real-estate-test-eb656",
  storageBucket: "real-estate-test-eb656.appspot.com",
  messagingSenderId: "83311609585",
  appId: "1:83311609585:web:9a99ba69b848914e840b50",
};

// Initialize Firebase
export const app = initializeApp(firebaseConfig);
