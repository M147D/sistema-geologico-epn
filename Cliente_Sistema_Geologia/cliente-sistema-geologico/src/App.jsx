// src/App.jsx
import { Route, createBrowserRouter, createRoutesFromElements, RouterProvider } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { ElementosProvider } from "./context/ElementosContext";
import RootLayout from "./layout/RootLayout.jsx";
import PageLogin from "./pages/PageLogin.jsx";
import PageMap from "./pages/PageMap.jsx";
import PageForm from "./pages/PageForm.jsx";
import PageDetails from "./pages/PageDetails.jsx";
import PageNotFound from "./pages/PageNotFound.jsx";
import PageTable from "./pages/PageTable.jsx";
import PageExcel from "./pages/PageExcel.jsx";
import PageUsers from "./pages/PageUsers.jsx";
import PageRegister from "./pages/PageRegister.jsx";
import ProtectedRoute from "./components/auth/ProtectedRoute.jsx";
import { ROLES } from "./constants/roles.js";

const App = () => {
  const router = createBrowserRouter(
    createRoutesFromElements(
      <>
        <Route path='/' element={<RootLayout />}>
          <Route index element={<PageLogin />} />
          <Route path='register' element={<PageRegister />} />
          <Route path='mapa' element={
            <ProtectedRoute>
              <PageMap />
            </ProtectedRoute>
          } />
          <Route path='crear-elementos' element={
            <ProtectedRoute allowedRoles={[ROLES.ADMIN, ROLES.INVITADO]}>
              <PageForm />
            </ProtectedRoute>
          } />
          <Route path='listar-elementos' element={
            <ProtectedRoute>
              <PageTable />
            </ProtectedRoute>
          } />
          <Route path="detalle/:id" element={
            <ProtectedRoute>
              <PageDetails />
            </ProtectedRoute>
          } />
          <Route path='carga-excel' element={
            <ProtectedRoute allowedRoles={[ROLES.ADMIN]}>
              <PageExcel />
            </ProtectedRoute>
          } />
          <Route path='usuarios' element={
            <ProtectedRoute allowedRoles={[ROLES.ADMIN]}>
              <PageUsers />
            </ProtectedRoute>
          } />
        </Route>
        <Route path='*' element={<PageNotFound />} />
      </>
    )
  );

  return (
    <AuthProvider>
      <ElementosProvider>
        <RouterProvider router={router} />
      </ElementosProvider>
    </AuthProvider>
  );
};

export default App;
