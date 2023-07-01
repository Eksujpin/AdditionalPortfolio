package dk.itu.moapd.scootersharing.fragments

import android.content.ContentValues
import android.location.Address
import android.location.Geocoder
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.*
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import dk.itu.moapd.scootersharing.R
import dk.itu.moapd.scootersharing.activities.ScooterSharingActivity
import dk.itu.moapd.scootersharing.models.Scooter
import dk.itu.moapd.scootersharing.models.ScooterVM
import java.io.IOException
import java.util.*
import kotlin.collections.ArrayList
import android.widget.ArrayAdapter
import dk.itu.moapd.scootersharing.databinding.FragmentEditScooterBinding


class EditScooterFragment : Fragment() {
    // GUI variables
    private lateinit var infoText: TextView
    private lateinit var nameText: EditText
    private lateinit var locationText: TextView
    private lateinit var scooter: Scooter
    private lateinit var spinner: Spinner
    private lateinit var spinnerState: String
    private var _binding: FragmentEditScooterBinding? = null
    private val binding get() = _binding!!
    private val scooterVM: ScooterVM by activityViewModels()

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View {
        _binding = FragmentEditScooterBinding.inflate(inflater, container, false)

        // Edit texts
        infoText = binding.infoText
        nameText = binding.nameText
        locationText = binding.LocationText
        spinner = binding.scooterSpinner

        scooterVM.getAll().observe(viewLifecycleOwner, { data ->
            val temp = data.toCollection(ArrayList())
            scooter = temp[ScooterSharingActivity.editThisScooter]
            spinnerState = scooter.scooterPicture.removeSuffix(".png")
            infoText.text = scooter.toString()
            ArrayAdapter.createFromResource(binding.root.context, R.array.scooter_models,android.R.layout.simple_spinner_item)
                .also { adapter ->
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                    spinner.adapter = adapter
                    if (spinnerState != null) {
                        val spinnerPosition = adapter.getPosition(spinnerState)
                        spinner.setSelection(spinnerPosition)
                    }
                }
        })

        val lat = scooterVM.locationState.value!!.latitude
        val long = scooterVM.locationState.value!!.longitude
        val address = getAddress(lat,long)
        locationText.text = address

        // Buttons
        binding.updateBtn.setOnClickListener {
            if (nameText.text.isNotEmpty()||spinner.selectedItem.toString() != spinnerState || address != scooter.scooterLocation){
                // Update the object attributes .
                if (nameText.text.isNotEmpty()) scooter.scooterName = nameText.text.toString().trim()
                scooter.scooterLocation = address
                scooter.scooterLat = lat
                scooter.scooterLong = long
                scooter.scooterDate = System.currentTimeMillis()
                scooter.scooterPicture = spinner.selectedItem.toString()+".png"
                scooterVM.update(scooter)
                // Reset the text fields and update the UI.
                nameText.text.clear()
                activity!!.supportFragmentManager.beginTransaction().replace(R.id.fragment_container, ScooterListFragment()).commit()
            }else Toast.makeText(this.requireContext(), "Pleas fill at least one field to edit this ride", Toast.LENGTH_SHORT).show()
        }

        //updating the UI so u know what you are editing
        return binding.root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    private fun getAddress(latitude: Double, longitude: Double): String {
        val geocoder = Geocoder(requireContext(), Locale.getDefault())
        try {
            val addresses = geocoder.getFromLocation(latitude, longitude, 1)
            return if (addresses.isNotEmpty()) {
                val address: Address = addresses[0]
                address.getAddressLine(0)
            } else "Address not found!"
        } catch (ex: IOException) { Log.e(ContentValues.TAG, ex.printStackTrace().toString()) }
        return "Address not found!"
    }

}