package dk.itu.moapd.scootersharing.activities

import android.Manifest
import android.annotation.SuppressLint
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Bundle
import android.os.Looper
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import com.google.android.gms.location.*
import com.google.firebase.auth.FirebaseAuth
import com.google.zxing.integration.android.IntentIntegrator
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.databinding.ActivityScooterSharingBinding
import dk.itu.moapd.scootersharing.fragments.*
import dk.itu.moapd.scootersharing.models.RideVM
import dk.itu.moapd.scootersharing.models.ScooterVM
import java.util.concurrent.TimeUnit


class ScooterSharingActivity : AppCompatActivity() {
    private lateinit var scooterSharing: Fragment
    private lateinit var createScooter: Fragment
    private lateinit var editScooter: Fragment
    private lateinit var userProfile: Fragment
    private lateinit var scooterList: Fragment
    private lateinit var rideFragment: Fragment
    private lateinit var maps: GoogleMapsFragment
    private lateinit var auth: FirebaseAuth
    private lateinit var fusedLocationProviderClient: FusedLocationProviderClient
    private lateinit var locationCallback: LocationCallback
    private lateinit var binding: ActivityScooterSharingBinding

    private val scooterViewModel: ScooterVM by lazy {
        ViewModelProvider(this)[ScooterVM::class.java]
    }
    private val rideViewModel: RideVM by lazy {
        ViewModelProvider(this)[RideVM::class.java]
    }

    companion object {
        private const val ALL_PERMISSIONS_RESULT = 1011
        var editThisScooter = 0
        var lastReserved: Int? = null
    }


    //folder layout -> https://developer.android.com/topic/libraries/architecture/coroutines
    override fun onStart() {
        super.onStart()
        if (auth.currentUser == null) startLoginActivity()
    }

    //overridden onCreate
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityScooterSharingBinding.inflate(layoutInflater)
        setContentView(binding.root)
        startLocationAware()

        // get user
        auth = FirebaseAuth.getInstance()

        scooterSharing = ScooterSharingFragment()
        createScooter = CreateScooterFragment()
        editScooter = EditScooterFragment()
        userProfile = UserProfileFragment()
        scooterList = ScooterListFragment()
        maps = GoogleMapsFragment()
        rideFragment = RideFragment()
        val currentFragment = supportFragmentManager.findFragmentById(R.id.fragment_container)
        if (currentFragment == null) {
            supportFragmentManager.beginTransaction().add(R.id.fragment_container, scooterSharing)
                .commit()
        }

        with(binding) {
            topAppBar.setNavigationOnClickListener {
                supportFragmentManager.beginTransaction()
                    .replace(R.id.fragment_container, scooterSharing).commit()
            }
            topAppBar.setOnMenuItemClickListener { menuItem ->
                when (menuItem.itemId) {
                    R.id.addScoter -> {
                        supportFragmentManager.beginTransaction()
                            .replace(R.id.fragment_container, createScooter).commit()
                        true
                    }
                    else -> false
                }
            }
        }

        binding.bottomNavigation.menu.getItem(0).isCheckable = false
        //bottom navigation behaviour
        with(binding) {
            bottomNavigation.setOnItemSelectedListener { item ->
                when (item.itemId) {

                    // Select the Fragment 1 bottom.
                    R.id.page_1 -> {
                        item.isCheckable = true
                        supportFragmentManager.beginTransaction()
                            .replace(R.id.fragment_container, maps).commit()
                        true
                    }

                    // Select the Fragment 2 bottom.
                    R.id.page_2 -> {
                        supportFragmentManager.beginTransaction()
                            .replace(R.id.fragment_container, scooterList).commit()
                        true
                    }

                    // Select the Fragment 3 bottom.
                    R.id.page_3 -> {
                        supportFragmentManager.beginTransaction()
                            .replace(R.id.fragment_container, rideFragment).commit()
                        true
                    }
                    // Select the Fragment 4 bottom.
                    R.id.page_4 -> {
                        supportFragmentManager.beginTransaction()
                            .replace(R.id.fragment_container, userProfile).commit()
                        true
                    }
                    else -> false
                }
            }
        }

    }

    override fun onResume() {
        super.onResume()
        subscribeToLocationUpdates()
    }

    override fun onPause() {
        super.onPause()
        unsubscribeToLocationUpdates()
    }

    private fun startLoginActivity() {
        val intent = Intent(this, LoginActivity::class.java)
        startActivity(intent)
        finish()
    }

    private fun requestUserPermissions() {
        // An array with location-aware permissions.
        val permissions: ArrayList<String> = ArrayList()
        permissions.add(Manifest.permission.ACCESS_FINE_LOCATION)
        permissions.add(Manifest.permission.ACCESS_COARSE_LOCATION)
        // Check which permissions is needed to ask to the user.
        val permissionsToRequest = permissionsToRequest(permissions)
        // Show the permissions dialogs to the user.
        if (permissionsToRequest.size > 0)
            requestPermissions(
                permissionsToRequest.toTypedArray(),
                ALL_PERMISSIONS_RESULT
            )
    }

    private fun permissionsToRequest(
        permissions: ArrayList<String>
    ): ArrayList<String> {
        val result: ArrayList<String> = ArrayList()
        for (permission in permissions)
            if (checkSelfPermission(permission) !=
                PackageManager.PERMISSION_GRANTED
            )
                result.add(permission)
        return result
    }

    private fun checkPermission() =
        ActivityCompat.checkSelfPermission(
            this, Manifest.permission.ACCESS_FINE_LOCATION
        ) == PackageManager.PERMISSION_GRANTED ||
                ActivityCompat.checkSelfPermission(
                    this, Manifest.permission.ACCESS_COARSE_LOCATION
                ) == PackageManager.PERMISSION_GRANTED

    @SuppressLint("MissingPermission")
    private fun subscribeToLocationUpdates() {
        // Check if the user allows the application to access the location-aware resources.
        if (!checkPermission())
            return

        // Sets the accuracy and desired interval for active location updates.
        val locationRequest = LocationRequest.create().apply {
            interval = TimeUnit.SECONDS.toMillis(5)
            fastestInterval = TimeUnit.SECONDS.toMillis(2)
            priority = LocationRequest.PRIORITY_HIGH_ACCURACY
        }
        // Subscribe to location changes.
        fusedLocationProviderClient.requestLocationUpdates(
            locationRequest,
            locationCallback,
            Looper.getMainLooper()
        )
    }

    private fun unsubscribeToLocationUpdates() {
        // Unsubscribe to location changes.
        fusedLocationProviderClient.removeLocationUpdates(locationCallback)
    }

    private fun startLocationAware() {
        requestUserPermissions()

        fusedLocationProviderClient = LocationServices
            .getFusedLocationProviderClient(this)

        // Initialize the `LocationCallback`.
        locationCallback = object : LocationCallback() {
            override fun onLocationResult(locationResult: LocationResult) {
                super.onLocationResult(locationResult)
                scooterViewModel.onLocationChanged(locationResult.lastLocation)
            }
        }
    }
}