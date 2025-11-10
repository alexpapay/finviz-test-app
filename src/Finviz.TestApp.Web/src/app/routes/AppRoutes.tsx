import { Routes, Route } from "react-router-dom";
import { ImageNetPage } from "@/pages/ImageNetBrowser/ui/ImageNetPage";
import { NotFoundPage } from "@/pages/NotFoundPage";

export const AppRoutes = () => (
    <Routes>
        <Route path="/" element={<ImageNetPage />} />
        <Route path="*" element={<NotFoundPage />} />
    </Routes>
);
