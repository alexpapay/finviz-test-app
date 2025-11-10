import { CircularProgress, Box } from "@mui/material";

export const Spinner = () => (
    <Box
        sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            p: 2
        }}
    >
        <CircularProgress />
    </Box>
);
