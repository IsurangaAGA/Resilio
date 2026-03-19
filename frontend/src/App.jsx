import { BrowserRouter, Routes, Route } from "react-router-dom";
import LandingPage from "./pages/LandingPage";
import OtpStartPage from "./pages/OtpStartPage";
import OtpVerifyPage from "./pages/OtpVerifyPage";
import VictimDashboard from "./pages/VictimDashboard";
import VolunteerDashboard from "./pages/VolunteerDashboard";
import ProtectedRoute from "./auth/ProtectedRoute";
import NewRequestPage from './pages/victim/NewRequestPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/auth/start" element={<OtpStartPage />} />
        <Route path="/auth/verify" element={<OtpVerifyPage />} />
        <Route path='/victim/new-request'element={<ProtectedRoute><NewRequestPage /></ProtectedRoute>} />
        

        <Route
          path="/victim"
          element={
            <ProtectedRoute>
              <VictimDashboard />
            </ProtectedRoute>
          }
        />

        <Route
          path="/volunteer"
          element={
            <ProtectedRoute>
              <VolunteerDashboard />
            </ProtectedRoute>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;