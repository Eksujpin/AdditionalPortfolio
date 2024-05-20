package dk.itu.moapd.scootersharing.fragments


import android.Manifest
import android.annotation.SuppressLint
import android.content.pm.PackageManager
import android.os.Bundle
import android.util.Log
import com.google.android.gms.maps.MapsInitializer.Renderer
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.app.ActivityCompat
import androidx.fragment.app.activityViewModels
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.OnMapsSdkInitializedCallback
import com.google.android.gms.maps.SupportMapFragment
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.MarkerOptions
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.databinding.FragmentGoogleMapsBinding
import dk.itu.moapd.scootersharing.models.ScooterVM
import java.util.*

class GoogleMapsFragment : Fragment(), OnMapsSdkInitializedCallback {

    private var _binding: FragmentGoogleMapsBinding? = null
    private val binding get() = _binding!!
    private val scooterVM: ScooterVM by activityViewModels()

    companion object {
        private val TAG = GoogleMapsFragment::class.qualifiedName
    }

    private val viewModel: ScooterVM by activityViewModels()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        // Inflate the layout for this fragment.
        _binding = FragmentGoogleMapsBinding.inflate(inflater, container, false)


        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        val mapsFragment = childFragmentManager.findFragmentById(R.id.map) as SupportMapFragment?
        mapsFragment?.getMapAsync(callback)
    }

    @SuppressLint("MissingPermission")
    private val callback = OnMapReadyCallback { googleMap ->
        val loc = LatLng(
            viewModel.locationState.value!!.latitude,
            viewModel.locationState.value!!.longitude
        )
        scooterVM.getAll().observe(viewLifecycleOwner, { data ->
            val scooters = data.toCollection(ArrayList())
            for (scooter in scooters) {
                val temp = LatLng(scooter.scooterLat,scooter.scooterLong)
                googleMap.addMarker(MarkerOptions().position(temp).title(scooter.scooterName))
            }
        })

        googleMap.moveCamera(CameraUpdateFactory.newLatLngZoom(loc, 18f))
        googleMap.mapType = GoogleMap.MAP_TYPE_NORMAL

        if (checkPermission()) {
            googleMap.isMyLocationEnabled = true
            googleMap.uiSettings.isMyLocationButtonEnabled = true
        }
    }

    override fun onMapsSdkInitialized(renderer: Renderer) {
        when (renderer) {
            Renderer.LATEST ->
                Log.d(TAG, "The latest version of the renderer is used.")
            Renderer.LEGACY ->
                Log.d(TAG, "The legacy version of the renderer is used.")
        }
    }

    private fun checkPermission() =
        ActivityCompat.checkSelfPermission(
            requireContext(), Manifest.permission.ACCESS_FINE_LOCATION
        ) == PackageManager.PERMISSION_GRANTED ||
                ActivityCompat.checkSelfPermission(
                    requireContext(), Manifest.permission.ACCESS_COARSE_LOCATION
                ) == PackageManager.PERMISSION_GRANTED


}