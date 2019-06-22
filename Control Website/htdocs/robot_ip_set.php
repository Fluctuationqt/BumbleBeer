<?php
	$newIP = $_GET['ip'];
	
	$servername = "localhost";
	$username = "root";
	$password = "";
	$dbname = "beerpong";

	// Create connection
	$conn = new mysqli($servername, $username, $password, $dbname);
	// Check connection
	if ($conn->connect_error) {
		die("Connection failed: " . $conn->connect_error);
	} 

	$sql = "UPDATE robotip SET address='".$newIP."' WHERE id=1";

	if ($conn->query($sql) === TRUE) {
		echo "Record updated successfully, ";
		echo "Robot ip was changed to $newIP";
	} else {
		echo "Error updating record: " . $conn->error;
	}
	
	$conn->close();
?>