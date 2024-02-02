import { useSelector } from "react-redux";
import { useEffect, useRef, useState } from "react";
import { setLogout } from "../redux/user/userSlice";
import { useDispatch } from "react-redux";
import { Link } from "react-router-dom";

import { app } from "../firebase";
import { getFirestore, collection, doc, updateDoc } from "firebase/firestore";
import {
  getStorage,
  ref,
  uploadBytesResumable,
  getDownloadURL,
} from "firebase/storage";

export default function Profile() {
  const fileInputRef = useRef(null);
  const dispatch = useDispatch();
  const { currentUser, error } = useSelector((state) => state.user);
  const [selectedFile, setSelectedFile] = useState(null);
  const [imagePreview, setImagePreview] = useState(currentUser.user.avatar);

  const [showListingsError, setShowListingsError] = useState(false);
  const [userListings, setUserListings] = useState([]);
  const db = getFirestore(app);

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    setSelectedFile(selectedFile);

    const reader = new FileReader();
    reader.onloadend = () => {
      setImagePreview(reader.result);
    };

    if (selectedFile) {
      reader.readAsDataURL(selectedFile);
    } else {
      setImagePreview();
    }
  };

  const handleImageUpload = async () => {
    try {
      if (selectedFile) {
        const storage = getStorage();
        const fileName = new Date().getTime() + selectedFile.name;
        const storageRef = ref(storage, "Profile_Pictures/" + fileName);

        const uploadTask = uploadBytesResumable(storageRef, selectedFile);

        uploadTask.on(
          "state_changed",
          (snapshot) => {
            const progress =
              (snapshot.bytesTransferred / snapshot.totalBytes) * 100;
            console.log(`Upload is ${progress}% done`);
          },
          (error) => {
            console.error("Error uploading image:", error);
          },
          async () => {
            try {
              const downloadURL = await getDownloadURL(uploadTask.snapshot.ref);
              console.log("File available at", downloadURL);

              const userDocRef = doc(
                collection(db, "UserData"),
                currentUser.user.id
              );
              await updateDoc(userDocRef, { Avatar: downloadURL });
            } catch (error) {
              console.error("Error getting download URL:", error);
            }
          }
        );
      }
    } catch (error) {
      console.error("Error uploading image:", error);
    }
  };

  const handleShowListings = async () => {
    try {
      setShowListingsError(false);
      const res = await fetch(
        `https://localhost:7029/api/Listings/getUserListings/${currentUser.user.id}`
      );
      const data = await res.json();
      if (data.success === false) {
        setShowListingsError(true);
        return;
      }

      setUserListings(data);
    } catch (error) {
      setShowListingsError(true);
    }
  };

  const handleListingDelete = async (listingId) => {
    try {
      const res = await fetch(
        `https://localhost:7029/api/Listings/delete/${listingId}`,
        {
          method: "DELETE",
        }
      );
      const data = await res.json();
      if (data.success === false) {
        console.log(data.message);
        return;
      }

      setUserListings((prev) =>
        prev.filter((listing) => listing.id !== listingId)
      );
    } catch (error) {
      console.log(error.message);
    }
  };

  return (
    <div className="p-3 max-w-lg mx-auto">
      <h1 className="text-3xl font-semibold text-center my-7">
        {currentUser.user.username}
      </h1>
      <h1 className="text-3xl font-semibold text-center my-7">
        {currentUser.user.email}
      </h1>

      {/* Profile Image */}
      <img
        onClick={() => fileInputRef.current.click()}
        src={imagePreview}
        alt="profile"
        className="mx-auto block rounded-full h-24 w-24 object-cover cursor-pointer self-center mt-2 mb-4 flex flex-col gap-4"
      />

      {/* Display selected file name */}
      {selectedFile && (
        <p className="text-slate-700 text-center">{`Selected File: ${selectedFile.name}`}</p>
      )}

      {/* Input for selecting a new image */}
      <input
        onChange={handleFileChange}
        type="file"
        ref={fileInputRef}
        hidden
        accept="image/*"
      />

      {/* Button to upload the new image */}
      <button
        onClick={handleImageUpload}
        className="bg-blue-500 text-white p-3 rounded-lg uppercase text-center hover:opacity-95 mt-2"
      >
        Upload Image
      </button>

      {/* Create Listing Link */}
      <Link
        className="bg-green-700 text-white p-3 rounded-lg uppercase text-center hover:opacity-95 flex flex-col gap-4 mt-4"
        to={"/create-listing"}
      >
        Create Listing
      </Link>

      {/* Sign Out */}
      <div className="flex justify-between mt-5">
        <span
          onClick={() => dispatch(setLogout())}
          className="text-red-700 cursor-pointer"
        >
          Sign out
        </span>
      </div>

      {/* Error Message */}
      <p className="text-red-700 mt-5">{error ? error : ""}</p>

      <button onClick={handleShowListings} className="text-green-700 w-full">
        Show Listings
      </button>
      <p className="text-red-700 mt-5">{/* Show Listings Error Message */}</p>

      {/* User Listings */}
      {userListings && userListings.length > 0 && (
        <div className="flex flex-col gap-4">
          <h1 className="text-center mt-7 text-2xl font-semibold">
            Your Listings
          </h1>
          {userListings.map((listing) => (
            <div
              key={listing.id}
              className="border rounded-lg p-3 flex justify-between items-center gap-4"
            >
              <Link to={`/listing/${listing.id}`}>
                <img
                  src={listing.imageUrls[0]}
                  alt="listing cover"
                  className="h-16 w-16 object-contain"
                />
              </Link>
              <Link
                className="text-slate-700 font-semibold  hover:underline truncate flex-1"
                to={`/listing/${listing.id}`}
              >
                <p>{listing.name}</p>
              </Link>

              <div className="flex flex-col item-center">
                <button
                  onClick={() => handleListingDelete(listing.id)}
                  className="text-red-700 uppercase"
                >
                  Delete
                </button>
                <Link to={`/update-listing/${listing.id}`}>
                  <button className="text-green-700 uppercase">Edit</button>
                </Link>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
