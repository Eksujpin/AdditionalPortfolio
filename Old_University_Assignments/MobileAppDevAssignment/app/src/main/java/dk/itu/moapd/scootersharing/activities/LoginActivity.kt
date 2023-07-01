package dk.itu.moapd.scootersharing.activities

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import com.firebase.ui.auth.AuthUI
import com.firebase.ui.auth.FirebaseAuthUIActivityResultContract
import com.firebase.ui.auth.data.model.FirebaseAuthUIAuthenticationResult
import com.google.firebase.auth.FirebaseAuth
import dk.itu.moapd.scootersharing.R

class LoginActivity  : AppCompatActivity() {

    private val signInLauncher = registerForActivityResult(
        FirebaseAuthUIActivityResultContract()
    ) { result ->
        onSignInResult(result)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        createSignInIntent()
    }


    private fun createSignInIntent() {

        // Choose authentication providers.
        val providers = arrayListOf(
            AuthUI.IdpConfig.EmailBuilder().build(),
            AuthUI.IdpConfig.FacebookBuilder().build(),
            AuthUI.IdpConfig.GoogleBuilder().build())

        // Create and launch sign-in intent.
        val signInIntent = AuthUI.getInstance()
            .createSignInIntentBuilder()
            .setAvailableProviders(providers)
            //.setLogo()
            .setTheme(R.style.Theme_ScooterSharing)
            .build()
        signInLauncher.launch(signInIntent)
    }

    private fun onSignInResult(result: FirebaseAuthUIAuthenticationResult) {
        if (result.resultCode == RESULT_OK) {
            val temp = FirebaseAuth.getInstance().currentUser?.displayName
            Toast.makeText(this, "$temp logged in the app.", Toast.LENGTH_SHORT).show()
            startMainActivity()
        } else
            Toast.makeText(this, "Authentication failed.", Toast.LENGTH_SHORT).show()

    }

    private fun startMainActivity() {
        val intent = Intent(this, ScooterSharingActivity::class.java)
        startActivity(intent)
        finish()
    }


}