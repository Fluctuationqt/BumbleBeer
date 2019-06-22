<?php
	/*echo "192.168.137.230";*/
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

	$sql = "SELECT address FROM robotip WHERE id=1";
	$result = $conn->query($sql)->fetch_assoc();
	echo $result["address"];
	
	$conn->close();
?>