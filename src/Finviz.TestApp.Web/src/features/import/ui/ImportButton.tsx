import {
    Button,
    CircularProgress,
    Snackbar,
    Alert,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogContentText,
    DialogActions,
    Typography,
    Box,
} from "@mui/material";
import { useState } from "react";
import { useImportImageNet } from "../model/useImportImageNet";

interface Props {
    onImportComplete?: () => void;
}

export const ImportButton = ({ onImportComplete }: Props) => {
    const { mutateAsync, isPending } = useImportImageNet();
    const [message, setMessage] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [confirmOpen, setConfirmOpen] = useState(false);
    const [lastImport, setLastImport] = useState<Date | null>(null);

    const handleImport = async () => {
        try {
            const result = await mutateAsync();
            setMessage(
                `✅ Imported ${result.totalSaved} of ${result.totalParsed} entries in ${result.durationMs} ms`
            );
            setLastImport(new Date());
            onImportComplete?.();
        } catch (err) {
            console.error(err);
            setError("Failed to import ImageNet structure.");
        }
    };

    return (
        <Box display="flex" flexDirection="column" alignItems="flex-end" gap={0.5}>
            <Button
                variant="contained"
                color="primary"
                onClick={() => setConfirmOpen(true)}
                disabled={isPending}
                startIcon={isPending ? <CircularProgress size={18} /> : undefined}
                sx={{ textTransform: "none", borderRadius: 2 }}
            >
                {isPending ? "Importing..." : "Import ImageNet"}
            </Button>

            {lastImport && (
                <Typography variant="caption" color="text.secondary">
                    Last import: {lastImport.toLocaleString()}
                </Typography>
            )}

            <Dialog open={confirmOpen} onClose={() => setConfirmOpen(false)}>
                <DialogTitle>Confirm Import</DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        This action will parse the ImageNet XML and insert all entries into
                        the database. It may take several seconds.
                        <br />
                        <br />
                        Are you sure you want to continue?
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setConfirmOpen(false)}>Cancel</Button>
                    <Button
                        color="primary"
                        variant="contained"
                        onClick={() => {
                            setConfirmOpen(false);
                            handleImport();
                        }}
                        autoFocus
                    >
                        Yes, Import
                    </Button>
                </DialogActions>
            </Dialog>

            <Snackbar
                open={!!message}
                autoHideDuration={4000}
                onClose={() => setMessage(null)}
            >
                <Alert
                    onClose={() => setMessage(null)}
                    severity="success"
                    variant="filled"
                >
                    {message}
                </Alert>
            </Snackbar>

            <Snackbar
                open={!!error}
                autoHideDuration={4000}
                onClose={() => setError(null)}
            >
                <Alert
                    onClose={() => setError(null)}
                    severity="error"
                    variant="filled"
                >
                    {error}
                </Alert>
            </Snackbar>
        </Box>
    );
};
